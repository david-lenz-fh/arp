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
                SELECT m.id, m.title, m.description, m.release_date, m.fsk, gm.genres, m.media_type, r.average_rating
                FROM media AS m
                LEFT JOIN (
                    SELECT media_id, COALESCE(array_agg(genre_name),'{}') AS genres FROM genre_media
                    GROUP BY media_id
                    ) AS gm ON m.id=gm.media_id
                LEFT JOIN (
                    SELECT media_id, AVG(rating.rating) AS average_rating FROM rating
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
            if (filter.MinRating != null)
            {
                sql.AppendLine("AND r.average_rating>=@minRating");
                sqlParams.Add("minRating", filter.MinRating);
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
            while (await reader.ReadAsync())
            {
                string? title = reader.IsDBNull(1)?null: reader.GetString(1);
                string? description = reader.IsDBNull(2) ? null : reader.GetString(2);
                DateOnly? release = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
                int? fsk = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                List<string> genres = reader.IsDBNull(5) ? new List<string>() : reader.GetFieldValue<string[]>(5).ToList();
                string? mediaType = reader.IsDBNull(6)?null: reader.GetString(6);
                double? averageRating = reader.IsDBNull(7) ? null : reader.GetDouble(7);

                re.Add(new MediaEntity(reader.GetInt32(0), title, description, release, fsk, genres, mediaType,averageRating));
            }
            return re;
        }
        public async Task<MediaEntity?> FindMediaById(int id)
        {
            String sql = """
                SELECT m.id, m.title, m.description, m.release_date, m.fsk, gm.genres, m.media_type, r.average_rating
                FROM media AS m
                LEFT JOIN (
                    SELECT media_id, COALESCE(array_agg(genre_name),'{}') AS genres FROM genre_media
                    GROUP BY media_id
                    ) AS gm ON m.id=gm.media_id
                LEFT JOIN (
                    SELECT media_id, AVG(rating.rating) AS average_rating FROM rating
                    GROUP BY media_id
                    ) AS r ON m.id=r.media_id
                WHERE m.id=@media_id 
                """;

            var sqlParams = new Dictionary<string, object?>
            {
                ["media_id"] = id,
            };
            var reader = await _postgres.SQLWithReturns(sql, sqlParams);
            if (await reader.ReadAsync())
            {
                string? title = reader.IsDBNull(1) ? null : reader.GetString(1);
                string? description = reader.IsDBNull(2) ? null : reader.GetString(2);
                DateOnly? release = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
                int? fsk = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                List<string> genres = reader.IsDBNull(5) ? new List<string>() : reader.GetFieldValue<string[]>(5).ToList();
                string? mediaType = reader.IsDBNull(6) ? null : reader.GetString(6);
                double? averageRating = reader.IsDBNull(7) ? null : reader.GetDouble(7);

                return new MediaEntity(reader.GetInt32(0), title, description, release, fsk, genres, mediaType, averageRating);
            }
            return null;
        }
        public async Task<List<string>> GetGenres()
        {
            var re=new List<string>();
            String sql = "SELECT genre_name FROM genre";
            var reader = await _postgres.SQLWithReturns(sql, new Dictionary<string, object?> { });
            while (await reader.ReadAsync())
            {
                re.Add(reader.GetString(0));
            }
            return re;
        }
        public async Task<int?> AddMedia(AddMedia media)
        {
            try
            {
                string sql = """
                    WITH new_media AS (
                        INSERT INTO media (title, description, release_date, fsk, media_type)
                        VALUES (@name, @description, @release_date, @fsk, @media_type)
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
                    ["fsk"]=media.Fsk, 
                    ["media_type"]=media.MediaType, 
                    ["genres"] = media.GenreNames.ToArray()
                };
                var reader = await _postgres.SQLWithReturns(sql, sqlParams);

                if (await reader.ReadAsync())
                {
                    return reader.GetFieldValue<int?>(0);
                }
                return null;
            }
            catch
            {
                return null;
            }
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
            sql.AppendLine("""UPDATE media SET id=id""");
            if (media.Title != null)
            {
                sql.AppendLine(""", title=@title""");
                sqlParams.Add("title", media.Title);
            }
            if (media.Description != null)
            {
                sql.AppendLine(""", description=@description""");
                sqlParams.Add("description", media.Description);
            }
            if (media.ReleaseDate != null)
            {
                sql.AppendLine(""", release_date=@releaseDate""");
                sqlParams.Add("releaseDate", media.ReleaseDate);
            }
            if (media.Fsk != null)
            {
                sql.AppendLine(""", fsk=@fsk""");
                sqlParams.Add("fsk", media.Fsk);
            }
            if (media.MediaType != null)
            {
                sql.AppendLine(""", media_type=@mediaType""");
                sqlParams.Add("mediaType", media.MediaType);
            }
            sql.AppendLine("""
                WHERE id=@media_id;
                COMMIT;
                """);
            Console.WriteLine(sql.ToString());
            int changedRow=await _postgres.SQLWithoutReturns(sql.ToString(), sqlParams);
            return changedRow >= 1;
        }
        public async Task<bool> DeleteMedia(int id)
        {
            try
            {
                string sql = """
                    WITH delete_genre AS (
                        DELETE FROM genre_media WHERE media_id=@id
                    )
                    DELETE FROM media WHERE id=@id
                    """;
                int changedRows = await _postgres.SQLWithoutReturns(sql, new Dictionary<string, object?> { ["id"] = id });
                return changedRows >= 1;
            }
            catch
            {
                return false;
            }
        }
    }
}
