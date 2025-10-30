

using DataAccess.Entities;

namespace DataAccess
{
    public class RatingRepository:IRatingRepository
    {
        PostgresDB _postgres_db;
        public RatingRepository(PostgresDB postgres)
        {
            _postgres_db = postgres;
        }

        public async Task<RatingEntity?> FindRatingById(int id)
        {
            string sql = "SELECT id, username, media_id, comment, stars FROM rating WHERE id=@id";
            var sqlParams = new Dictionary<string, object?> { ["id"] = id };
            var reader=await _postgres_db.SQLWithReturns(sql, sqlParams);
            if(await reader.ReadAsync())
            {
                return new RatingEntity(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2),
                                        reader.GetString(3), reader.GetInt32(4));
            }
            return null;
        }
        public async Task<int?> AddRating(AddRating added)
        {
            string sql = """
                INSERT INTO rating (username, media_id, comment, stars) 
                VALUES (@username, @media_id, @comment, @stars)
                RETURNING rating_id
                """;
            var sqlParams = new Dictionary<string, object?> {
                ["username"] = added.Username,
                ["media_id"] = added.MediaId,
                ["comment"] = added.Comment,
                ["stars"] = added.Stars
                };
            var reader = await _postgres_db.SQLWithReturns(sql, sqlParams);
            if (!await reader.ReadAsync())
            {
                return null;
            }
            return reader.GetFieldValue<int?>(0);
        }
        public async Task<bool> UpdateRating(UpdateRating updated)
        {
            string sql = """
                UPDATE rating 
                SET comment=COALESCE(@comment, comment),
                    stars=COALESCE(@stars, stars)
                WHERE id=@id
                """;

            var sqlParams = new Dictionary<string, object?> { 
                ["comment"] = updated.Comment, 
                ["stars"] = updated.Stars, 
                ["id"] = updated.Id
            };
            int changedRows = await _postgres_db.SQLWithoutReturns(sql, sqlParams);
            return changedRows>0;
        }
        public async Task<bool> DeleteRating(UpdateRating updated)
        {
            string sql = """DELETE FROM rating WHERE id=@id""";
            var sqlParams = new Dictionary<string, object?> { ["id"] = updated.Id };
            int changedRows = await _postgres_db.SQLWithoutReturns(sql, sqlParams);
            return changedRows > 0;
        }
        public async Task<bool> Favourite(string username, int mediaId)
        {
            string sql = """
                INSERT INTO favourite (username, media_id) 
                VALUES (@username, @media_id)
                """;
            var sqlParams = new Dictionary<string, object?> {
                ["username"] = username,
                ["media_id"] = mediaId
            };
            int changedRows = await _postgres_db.SQLWithoutReturns(sql, sqlParams);
            return changedRows > 0;
        }
        public async Task<bool> Unfavourite(string username, int mediaId)
        {
            string sql = """
                DELETE FROM favourite
                WHERE username=@username AND media_id=@media_id
                """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["username"] = username,
                ["media_id"] = mediaId
            };
            int changedRows = await _postgres_db.SQLWithoutReturns(sql, sqlParams);
            return changedRows > 0;
        }

        public async Task<List<RatingEntity>> GetRatingsForUser(string username)
        {
            var re = new List<RatingEntity>();
                string sql = """
                SELECT rating_id, username, media_id, "comment", stars FROM rating 
                WHERE username=@username
                """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["username"] = username
            };
            var reader=await _postgres_db.SQLWithReturns(sql, sqlParams);
            while (await reader.ReadAsync())
            {
                string? comment = reader.IsDBNull(3) ? null : reader.GetString(3);
                int? rating = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                re.Add(new RatingEntity(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), comment, rating));
            }
            return re;
        }

        public async Task<List<FavouriteEntity>> GetFavourites(string username)
        {
            var re = new List<FavouriteEntity>();
            string sql = """
                SELECT media_id FROM favourite 
                WHERE username=@username
                """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["username"] = username
            };
            var reader = await _postgres_db.SQLWithReturns(sql, sqlParams);
            while (await reader.ReadAsync())
            {
                re.Add(new FavouriteEntity(username, reader.GetInt32(0)));
            }
            return re;
        }
    }
}
        