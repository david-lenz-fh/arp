using MRP.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRP.Data
{
    internal class MediaRepository
    {
        PostgresDB _postgres;
        public MediaRepository(PostgresDB postgres)
        {
            _postgres = postgres;
        }
        public async Task<List<Media>> GetMedia()
        {
            var re=new List<Media>();
            String sql = """
                SELECT media.id, media.name, media.description, media.release_date, media.fsk, COALESCE(array_agg(genre.name), '{}') AS genre_ids, 
                COALESCE(array_agg(genre.name), '{}') AS genre_names, media_type.id AS type_id, media_type.name AS type_name
                FROM media
                LEFT JOIN media_genre ON media.id=media_genre.media_id
                LEFT JOIN genre ON media_genre.genre_id=genre.id
                LEFT JOIN media_type ON media.media_type_id=media_type.id
                GROUP BY media.id, media.name, media.description, media.release_date, media.fsk, media_type.name, media_type.id
                """;
            var reader = await _postgres.SQLWithReturns(sql, new Dictionary<string, object> { });
            while (await reader.ReadAsync())
            {
                re.Add(new Media(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetInt32(4),
                                 TransformToGenre(reader.GetFieldValue<int[]>(5), reader.GetFieldValue<string[]>(6)), new MediaType(reader.GetInt32(7), reader.GetString(8))));
            }
            return re;
        }
        public async Task<Media?> FindById(int mediaId)
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


            var reader = await _postgres.SQLWithReturns(sql, new Dictionary<string, object> { ["@media_id"] = mediaId });
            if (await reader.ReadAsync())
            {
                return new Media(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetInt32(4),
                                TransformToGenre(reader.GetFieldValue<int[]>(5),reader.GetFieldValue<string[]>(6)), new MediaType(reader.GetInt32(7), reader.GetString(8)));
            }
            return null;
        }
        public async Task<List<string>> GetGenres()
        {
            var re=new List<string>();
            String sql = "SELECT name FROM genre";
            var reader = await _postgres.SQLWithReturns(sql, new Dictionary<string, object> { });
            while (await reader.ReadAsync())
            {
                re.Add(reader.GetString(0));
            }
            return re;
        }
        public async Task<List<string>> GetMediaTypes()
        {
            var re = new List<string>();
            String sql = "SELECT name FROM media_type";
            var reader = await _postgres.SQLWithReturns(sql, new Dictionary<string, object> { });
            while (await reader.ReadAsync())
            {
                re.Add(reader.GetString(0));
            }
            return re;
        }
        public async Task<Media?> AddMedia(Media media)
        {
            try
            {
                String sql = """
                    WITH new_media AS (
                        INSERT INTO media (name, description, release_date, fsk, media_type_id)
                        VALUES (@name, @description, @release_date, @fsk, @media_type_id)
                        RETURNING id
                    )
                    INSERT INTO media_genre (media_id, genre_id)
                    SELECT new_media.id, genre.id FROM new_media
                    JOIN genre ON genre.id IN unnest(@genre_ids)
                    RETURNING media_id
                    """;
                int[] genreIds = media.Genres.Select(genre => genre.Id).ToArray();
                var sqlParams = new Dictionary<string, object> { ["@name"] = media.Title, ["@description"] = media.Description, 
                    ["@release_date"] = media.ReleaseDate, ["@fsk"]=media.Fsk, ["@media_type_id"]=media.MediaType.Id, ["@genre_ids"] = genreIds };

                var reader = await _postgres.SQLWithReturns(sql, sqlParams);

                if (await reader.ReadAsync())
                {
                    return media with { Id = reader.GetInt32(0) };
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> Delete(int Id)
        {
            try
            {
                int changedRow = await _postgres.SQLWithoutReturns("DELETE FROM mrp_user WHERE user_id=@id", new Dictionary<string, object> { ["id"] = Id });
                if (changedRow == 1)
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
        private static List<Genre> TransformToGenre(int[] ids, string[] names)
        {
            var re=new List<Genre>();
            for(int i=0; i<ids.Length; i++)
            {
                re.Add(new Genre(ids[i], names[i]));
            }
            return re;
        }
    }
}
