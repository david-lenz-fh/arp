using System.Data;
using System.Text;
using DataAccess.Entities;
namespace DataAccess
{
    public class MediaRepository:IMediaRepository
    {
        private PostgresDB _postgres;
        public MediaRepository(PostgresDB postgres)
        {
            _postgres = postgres;
        }
        public async Task<List<MediaEntity>> GetMedia(MediaFilterDAL filter)
        {
            
            StringBuilder sql=new StringBuilder("""
                SELECT m.id, m.title, m.description, m.release_date, m.fsk, gm.genres, m.media_type, r.average_stars, m.creator
                FROM media AS m
                LEFT JOIN (
                    SELECT media_id, COALESCE(array_agg(genre_name),'{}') AS genres FROM genre_media
                    GROUP BY media_id
                    ) AS gm ON m.id=gm.media_id
                LEFT JOIN (
                    SELECT media_id, AVG(rating.stars) AS average_stars FROM rating
                    GROUP BY media_id
                    ) AS r ON m.id=r.media_id
                WHERE TRUE                
                """);
            var sqlParams = new Dictionary<string, object?>();

            if (filter.Title != null)
            {
                sql.AppendLine("AND m.title=@title");
                sqlParams.Add("title", filter.Title);
            }
            if (filter.MediaType != null)
            {
                sql.AppendLine("AND m.media_type=@mediaType");
                sqlParams.Add("mediaType", filter.MediaType);
            }
            if (filter.Fsk != null)
            {
                sql.AppendLine("AND m.fsk<=@fsk");
                sqlParams.Add("fsk", filter.Fsk);
            }
            if (filter.MinStars != null)
            {
                sql.AppendLine("AND r.average_stars>=@minStars");
                sqlParams.Add("minStars", filter.MinStars);
            }
            if (filter.Genre != null)
            {
                sql.AppendLine("AND @genre=ANY(gm.genres)");
                sqlParams.Add("genre", filter.Genre);
            }
            if (filter.ReleaseYear != null)
            {
                try
                {
                    var startReleaseFrame = DateOnly.Parse(filter.ReleaseYear + "-1-1");
                    var endReleaseFrame = DateOnly.Parse(filter.ReleaseYear + "-12-31");
                    sql.AppendLine("AND m.release_date>=@startReleaseFrame");
                    sql.AppendLine("AND m.release_date<=@endReleaseFrame");
                    sqlParams.Add("startReleaseFrame", startReleaseFrame);
                    sqlParams.Add("endReleaseFrame", endReleaseFrame);
                }
                catch (Exception)
                {

                }
            }
            if (filter.SortBy != null)
            {
                switch (filter.SortBy)
                {
                    case "title":
                        sql.AppendLine("ORDER BY m.title");
                        break;
                    case "year":
                        sql.AppendLine("ORDER BY m.release_date");
                        break;
                    case "score":
                        sql.AppendLine("ORDER BY m.release_date");
                        break;
                }
            }
            var re=new List<MediaEntity>();            
            var reader = await _postgres.SQLWithReturns(sql.ToString(), sqlParams);
            while (reader != null && await reader.ReadAsync())
            {
                string? title = reader.IsDBNull(1)?null: reader.GetString(1);
                string? description = reader.IsDBNull(2) ? null : reader.GetString(2);
                DateOnly? release = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
                int? fsk = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                List<string> genres = reader.IsDBNull(5) ? new List<string>() : reader.GetFieldValue<string[]>(5).ToList();
                string? mediaType = reader.IsDBNull(6)?null: reader.GetString(6);
                double? averageRating = reader.IsDBNull(7) ? null : reader.GetDouble(7);
                string creatorName = reader.GetString(8);

                re.Add(new MediaEntity(reader.GetInt32(0), title, description, release, fsk, genres, mediaType, averageRating, creatorName));
            }
            return re;
        }
        public async Task<MediaEntity?> FindMediaById(int id)
        {
            String sql = """
                SELECT m.id, m.title, m.description, m.release_date, m.fsk, gm.genres, m.media_type, r.average_stars, m.creator
                FROM media AS m
                LEFT JOIN (
                    SELECT media_id, COALESCE(array_agg(genre_name),'{}') AS genres FROM genre_media
                    GROUP BY media_id
                    ) AS gm ON m.id=gm.media_id
                LEFT JOIN (
                    SELECT media_id, AVG(rating.stars) AS average_stars FROM rating
                    GROUP BY media_id
                    ) AS r ON m.id=r.media_id
                WHERE m.id=@media_id 
                """;

            var sqlParams = new Dictionary<string, object?>
            {
                ["media_id"] = id,
            };
            var reader = await _postgres.SQLWithReturns(sql, sqlParams);
            if (reader == null || !await reader.ReadAsync())
            {
                return null;
            }
            string? title = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? description = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateOnly? release = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
            int? fsk = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            List<string> genres = reader.IsDBNull(5) ? new List<string>() : reader.GetFieldValue<string[]>(5).ToList();
            string? mediaType = reader.IsDBNull(6) ? null : reader.GetString(6);
            double? averageRating = reader.IsDBNull(7) ? null : reader.GetDouble(7);
            string creatorName = reader.GetString(8);

            return new MediaEntity(reader.GetInt32(0), title, description, release, fsk, genres, mediaType, averageRating,creatorName);
        }
        public async Task<List<string>> GetGenres()
        {
            var re=new List<string>();
            String sql = "SELECT genre_name FROM genre";
            var reader = await _postgres.SQLWithReturns(sql, new Dictionary<string, object?> { });
            while (reader!=null&&await reader.ReadAsync())
            {
                re.Add(reader.GetString(0));
            }
            return re;
        }
        public async Task<int?> AddMedia(AddMedia media)
        {
            string sql = """
                WITH new_media AS (
                    INSERT INTO media (title, description, release_date, fsk, media_type, creator)
                    VALUES (@name, @description, @release_date, @fsk, @media_type, @creatorName)
                    RETURNING id
                )
                INSERT INTO genre_media (media_id, genre_name)
                SELECT new_media.id, g.genre FROM new_media
                CROSS JOIN unnest(@genres) AS g(genre)
                RETURNING media_id
                """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["name"] = media.Title,
                ["description"] = media.Description,
                ["release_date"] = media.ReleaseDate,
                ["fsk"] = media.Fsk,
                ["media_type"] = media.MediaType,
                ["genres"] = media.GenreNames.ToArray(),
                ["creatorName"] = media.CreatorName
            };
            var reader = await _postgres.SQLWithReturns(sql, sqlParams);

            if (reader==null||!await reader.ReadAsync())
            {
                return null;
            }
            return reader.GetFieldValue<int?>(0);
        }
        public async Task<bool> UpdateMedia(UpdateMedia media) { 
            StringBuilder sql = new StringBuilder("BEGIN;\n");
            var sqlParams = new Dictionary<string, object?>
            {
                ["media_id"] = media.Id
            };
            if (media.Genres!=null)
            {
                string genreSql = """

                    DELETE FROM genre_media
                    WHERE media_id=@media_id;

                    INSERT INTO genre_media (media_id, genre_name)
                    SELECT @media_id, unnest(@genre);

                    """;
                sql.AppendLine(genreSql);
                sqlParams.Add("genre", media.Genres);
            }
            sql.Append("""
                UPDATE media 
                SET id=id
                """);
            if (media.Title != null)
            {
                sql.Append(",\ntitle=@title");
                sqlParams.Add("title", media.Title);
            }
            if (media.Description != null)
            {
                sql.Append(",\ndescription=@description");
                sqlParams.Add("description", media.Description);
            }
            if (media.ReleaseDate != null)
            {
                sql.Append(",\nrelease_date=@releaseDate");
                sqlParams.Add("releaseDate", media.ReleaseDate);
            }
            if (media.Fsk != null)
            {
                sql.Append(",\nfsk=@fsk");
                sqlParams.Add("fsk", media.Fsk);
            }
            if (media.MediaType != null)
            {
                sql.Append(",\nmedia_type=@mediaType");
                sqlParams.Add("mediaType", media.MediaType);
            }
            sql.AppendLine("""
                
                WHERE id=@media_id;

                COMMIT;
                """);
            int changedRow=await _postgres.SQLWithoutReturns(sql.ToString(), sqlParams);
            return changedRow >= 1;
        }
        public async Task<bool> DeleteMedia(int id)
        {
            string sql = """
                BEGIN;
                DELETE FROM favourite WHERE media_id=@id;
                DELETE FROM genre_media WHERE media_id=@id;
                DELETE FROM rating WHERE media_id=@id;
                DELETE FROM media WHERE id=@id;
                COMMIT;
                """;
            int changedRows = await _postgres.SQLWithoutReturns(sql, new Dictionary<string, object?> { ["id"] = id });
            return changedRows >= 1;
            
        }
    }
}
