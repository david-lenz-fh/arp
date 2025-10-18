using System.Data;
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
        public async Task<List<MediaEntity>> GetMedia()
        {
            var re=new List<MediaEntity>();
            String sql = """
                SELECT media.id, media.name, media.description, media.release_date, media.fsk, COALESCE(array_agg(genre.name), '{}') AS genre_ids, 
                COALESCE(array_agg(genre.name), '{}') AS genre_names, media_type.id AS type_id, media_type.name AS type_name
                FROM media
                LEFT JOIN media_genre ON media.id=media_genre.media_id
                LEFT JOIN genre ON media_genre.genre_id=genre.id
                LEFT JOIN media_type ON media.media_type_id=media_type.id
                GROUP BY media.id, media.name, media.description, media.release_date, media.fsk, media_type.name, media_type.id
                """;
            var reader = await _postgres.SQLWithReturns(sql, new Dictionary<string, object?> { });
            while (await reader.ReadAsync())
            {
                re.Add(new MediaEntity(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetInt32(4),
                                 TransformToGenre(reader.GetFieldValue<int[]>(5), reader.GetFieldValue<string[]>(6)), new MediaTypeEntity(reader.GetInt32(7), reader.GetString(8))));
            }
            return re;
        }
        public async Task<MediaEntity?> FindMediaById(int id)
        {
            String sql = """
                SELECT media.id, media.name, media.description, media.release_date, media.fsk, COALESCE(array_agg(genre.name), '{}') AS genre_ids, 
                COALESCE(array_agg(genre.name), '{}') AS genre_names, media_type.id AS type_id, media_type.name AS type_name
                FROM media
                LEFT JOIN media_genre ON media.id=media_genre.media_id
                LEFT JOIN genre ON media_genre.genre_id=genre.id
                LEFT JOIN media_type ON media.media_type_id=media_type.id
                WHERE media.id=@media_id
                GROUP BY media.id, media.name, media.description, media.release_date, media.fsk, media_type.name, media_type.id
                """;

            var sqlParams = new Dictionary<string, object?>
            {
                ["media_id"] = id,
            };
            var reader = await _postgres.SQLWithReturns(sql, sqlParams);
            if (await reader.ReadAsync())
            {
                return new MediaEntity(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), 
                                       reader.GetInt32(4), TransformToGenre(reader.GetFieldValue<int[]>(5),
                                       reader.GetFieldValue<string[]>(6)), new MediaTypeEntity(reader.GetInt32(7), reader.GetString(8)));
            }
            return null;
        }
        public async Task<GenreEntity?> FindGenreById(int id)
        {
            String sql = "SELECT name_eng FROM genre WHERE id=@id";
            var sqlParams = new Dictionary<string, object?>
            {
                ["id"] = id,
            };
            var reader = await _postgres.SQLWithReturns(sql, sqlParams);
            if (await reader.ReadAsync())
            {
                return new GenreEntity(id, reader.GetString(0));
            }
            return null;
        }
        public async Task<List<GenreEntity>> GetGenres()
        {
            var re=new List<GenreEntity>();
            String sql = "SELECT id, name FROM genre";
            var reader = await _postgres.SQLWithReturns(sql, new Dictionary<string, object?> { });
            while (await reader.ReadAsync())
            {
                re.Add(new GenreEntity(reader.GetInt32(0), reader.GetString(1)));
            }
            return re;
        }
        public async Task<List<MediaTypeEntity>> GetMediaTypes()
        {
            var re = new List<MediaTypeEntity>();
            String sql = "SELECT name FROM media_type";
            var reader = await _postgres.SQLWithReturns(sql, new Dictionary<string, object?> { });
            while (await reader.ReadAsync())
            {
                re.Add(new MediaTypeEntity(reader.GetInt32(0), reader.GetString(1)));
            }
            return re;
        }
        public async Task<int?> AddMedia(AddMedia media)
        {
            try
            {
                string sql = """
                    WITH new_media AS (
                        INSERT INTO media (name, description, release_date, fsk, media_type_id)
                        VALUES (@name, @description, @release_date, @fsk, @media_type_id)
                        RETURNING id
                    )
                    INSERT INTO genre_media (media_id, genre_id)
                    SELECT new_media.id, genre.id FROM new_media
                    JOIN genre ON genre.id = ANY(@genre_ids)
                    RETURNING media_id
                    """;
                var sqlParams = new Dictionary<string, object?> 
                { 
                    ["name"] = media.Title, 
                    ["description"] = media.Description, 
                    ["release_date"] = media.ReleaseDate, 
                    ["fsk"]=media.Fsk, 
                    ["media_type_id"]=media.MediaTypeId, 
                    ["genre_ids"] = media.GenreIds 
                };

                var reader = await _postgres.SQLWithReturns(sql, sqlParams);

                if (await reader.ReadAsync())
                {
                    return reader.GetInt32(0);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> UpdateMedia(UpdateMedia media)
        {
            string sql = """
                WITH updated_genre AS(
                    UPDATE genre_media
                    SET COALESCE(@genre_id, genre_id)
                    WHERE media_id = @media_id
                )
                UPDATE media 
                SET COALESCE(@name, name),
                    COALESCE(@description, description),
                    COALESCE(@release_date, release_date),
                    COALESCE(@fsk, fsk),
                    COALESCE(@media_type_id, media_type_id)
                WHERE id=@media_id
                """;
            var sql_params = new Dictionary<string, object?> {
                ["media_id"] = media.Id,
                ["name"] = media.Title,
                ["description"] = media.Description,
                ["release_date"] = media.ReleaseDate,
                ["fsk"] = media.Fsk,
                ["media_type_id"] = media.MediaType?.Id
            };
            int changedRow=await _postgres.SQLWithoutReturns(sql, sql_params);
            if (changedRow >= 1)
            {
                return true;
            }
            return false;
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
                int changedRow = await _postgres.SQLWithoutReturns(sql, new Dictionary<string, object?> { ["id"] = id });
                if (changedRow >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private static List<GenreEntity> TransformToGenre(int[] ids, string[] names)
        {
            var re=new List<GenreEntity>();
            for(int i=0; i<ids.Length; i++)
            {
                re.Add(new GenreEntity(ids[i], names[i]));
            }
            return re;
        }
    }
}
