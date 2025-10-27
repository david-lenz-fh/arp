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
                SELECT media.id, media.title, media.description, media.release_date, media.fsk,  
                COALESCE(array_agg(genre.genre_name), '{}') AS genre_names, media_type.name AS type_name
                FROM media
                LEFT JOIN genre_media ON media.id=genre_media.media_id
                LEFT JOIN genre ON genre_media.genre_name=genre.genre_name
                LEFT JOIN media_type ON media.media_type=media_type.name
                GROUP BY media.id, media.title, media.description, media.release_date, media.fsk, media_type.name
                """;
            var reader = await _postgres.SQLWithReturns(sql, new Dictionary<string, object?> { });
            while (await reader.ReadAsync())
            {
                re.Add(new MediaEntity(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetFieldValue<DateOnly>(3), reader.GetInt32(4),
                                  reader.GetFieldValue<string[]>(5).ToList(), reader.GetString(6)));
            }
            return re;
        }
        public async Task<MediaEntity?> FindMediaById(int id)
        {
            String sql = """
                SELECT media.id, media.title, media.description, media.release_date, media.fsk,  
                COALESCE(array_agg(genre.genre_name), '{}') AS genre_names, media_type.name AS type_name
                FROM media
                LEFT JOIN genre_media ON media.id=genre_media.media_id
                LEFT JOIN genre ON genre_media.genre_name=genre.genre_name
                LEFT JOIN media_type ON media.media_type=media_type.name
                WHERE media.id=@media_id
                GROUP BY media.id, media.title, media.description, media.release_date, media.fsk, media_type.name
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
                List<string> genres = reader.GetFieldValue<string[]>(5).ToList();
                string? mediaType = reader.IsDBNull(6) ? null : reader.GetString(6);

                return new MediaEntity(reader.GetInt32(0), title, description, release, fsk, genres,  mediaType);
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
                    COALESCE(@media_type, media_type)
                WHERE id=@media_id
                """;
            var sql_params = new Dictionary<string, object?> {
                ["media_id"] = media.Id,
                ["name"] = media.Title,
                ["description"] = media.Description,
                ["release_date"] = media.ReleaseDate,
                ["fsk"] = media.Fsk,
                ["media_type"] = media.MediaType
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
    }
}
