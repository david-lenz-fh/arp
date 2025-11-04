using DataAccess.Entities;

namespace DataAccess
{
    public class UserRepository:IUserRepository
    {
        PostgresDB _postgres;
        public UserRepository(PostgresDB postgres)
        {
            _postgres = postgres;
        }

        public async Task<UserEntity?> FindUserByName(string username)
        {
            string sql = """
            SELECT username, password, email, favourite_genre_name 
            FROM mrp_user WHERE username=@username
            """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["username"] = username
            };
            var reader = await _postgres.SQLWithReturns(sql, sqlParams);
            if (reader == null || !await reader.ReadAsync())
            {
                return null;
            }
            string? email = reader.IsDBNull(2) ? null : reader.GetString(2);
            string? favouriteGenre = reader.IsDBNull(3) ? null : reader.GetString(3);
            return new UserEntity(reader.GetString(0), reader.GetString(1),email,favouriteGenre); 
        }
        public async Task<bool> AddUser(UserEntity user)
        {
            string sql = """
                INSERT INTO mrp_user (username, password) 
                VALUES (@name, @pwd)
                """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["name"] = user.Username,
                ["pwd"] = user.Password
            };
            var changedRows = await _postgres.SQLWithoutReturns(sql, sqlParams);

            return changedRows > 0;
        }
        public async Task<bool> DeleteUser(string username)
        {
            string sql = """
                DELETE FROM mrp_user 
                WHERE username=@username
                """;
            var sqlParams = new Dictionary<string, object?> { 
                ["username"] = username
            };
            int changedRow=await _postgres.SQLWithoutReturns(sql, sqlParams);
            return changedRow > 0;
        }
        public async Task<bool> UpdateUser(UserEntity updated)
        {
            string sql = """
                UPDATE mrp_user 
                SET password = @password, email=@email, favourite_genre_name=@favouriteGenre
                WHERE username = @username
                """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["username"] = updated.Username,
                ["password"] = updated.Password,
                ["favouriteGenre"] = updated.FavouriteGenre,
                ["email"] = updated.Email
            };

            int changedRow = await _postgres.SQLWithoutReturns(sql, sqlParams);
            return changedRow > 0;
         }
    }
}