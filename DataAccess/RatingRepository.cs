

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
            string sql = "SELECT rating_id, username, media_id, comment, stars, confirmed, timestamp FROM rating WHERE rating_id=@id";
            var sqlParams = new Dictionary<string, object?> { ["id"] = id };
            await using var reader=await _postgres_db.SQLWithReturns(sql, sqlParams);
            if (reader == null || !await reader.ReadAsync())
            {
                return null;
            }
            int ratingId = reader.GetInt32(0);
            string username = reader.GetString(1);
            int mediaId = reader.GetInt32(2);
            string? comment = reader.IsDBNull(3) ? null : reader.GetString(3);
            int stars = reader.GetInt32(4);
            bool confirmed = reader.GetBoolean(5);
            DateTime? timestamp = reader.IsDBNull(6)?null: reader.GetDateTime(6);

            return new RatingEntity(ratingId, username, mediaId, comment, stars, confirmed, timestamp);
            
        }
        public async Task<int?> AddRating(AddRating added)
        {
            string sql = """
                INSERT INTO rating (username, media_id, comment, stars, confirmed, timestamp) 
                VALUES (@username, @media_id, @comment, @stars, false, @timestamp)
                RETURNING rating_id
                """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["username"] = added.Username,
                ["media_id"] = added.MediaId,
                ["comment"] = added.Comment,
                ["stars"] = added.Stars,
                ["timestamp"] = DateTime.UtcNow
            };
            await using var reader = await _postgres_db.SQLWithReturns(sql, sqlParams);
            if (reader == null || !await reader.ReadAsync())
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
                    stars=COALESCE(@stars, stars),
                    confirmed=COALESCE(@confirmed, confirmed)
                WHERE rating_id=@id
                """;

            var sqlParams = new Dictionary<string, object?> { 
                ["comment"] = updated.Comment, 
                ["stars"] = updated.Stars,
                ["confirmed"] = updated.Confirmed,
                ["id"] = updated.Id
            };
            int changedRows = await _postgres_db.SQLWithoutReturns(sql, sqlParams);
            return changedRows>0;
        }
        public async Task<bool> DeleteRatingById(int id)
        {
            string sql = """
                BEGIN;
                DELETE FROM rating_likes WHERE rating_id=@id;
                DELETE FROM rating WHERE rating_id=@id;
                COMMIT;
                """;
            var sqlParams = new Dictionary<string, object?> { 
                ["id"] = id 
            };
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
                SELECT rating_id, media_id, "comment", stars, confirmed, timestamp FROM rating 
                WHERE username=@username
                """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["username"] = username
            };
            await using var reader=await _postgres_db.SQLWithReturns(sql, sqlParams);
            while (reader != null && await reader.ReadAsync())
            {
                int ratingId = reader.GetInt32(0);
                int mediaId = reader.GetInt32(1);
                string? comment = reader.IsDBNull(2) ? null : reader.GetString(2);
                int stars = reader.GetInt32(3);
                bool confirmed = reader.GetBoolean(4);
                DateTime? timestamp = reader.IsDBNull(5) ? null : reader.GetDateTime(5);
                re.Add(new RatingEntity(ratingId, username, mediaId , comment, stars, confirmed, timestamp));
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
            await using var reader = await _postgres_db.SQLWithReturns(sql, sqlParams);
            while (reader!=null && await reader.ReadAsync())
            {
                re.Add(new FavouriteEntity(username, reader.GetInt32(0)));
            }
            return re;
        }

        public async Task<bool> LikeRating(string username, int ratingId)
        {
            string sql = """
                INSERT INTO rating_likes (username, rating_id)
                VALUES (@username, @rating_id)
                """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["username"] = username,
                ["rating_id"] = ratingId
            };
            int changedRows = await _postgres_db.SQLWithoutReturns(sql, sqlParams);
            return changedRows > 0;
        }
    }
}
        