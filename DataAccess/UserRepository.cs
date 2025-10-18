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
            SELECT username, password, email, favorite_genre_id 
            FROM mrp_user WHERE username=@username
            """;
            var sqlParams = new Dictionary<string, object?>
            {
                ["username"] = username
            };
            var reader = await _postgres.SQLWithReturns(sql, sqlParams);
            if (await reader.ReadAsync())
            {
                string? email = reader.IsDBNull(2) ? null : reader.GetString(2);
                return new UserEntity(reader.GetString(0), reader.GetString(1),email,reader.GetFieldValue<int?>(3));
            }
            return null;
        }
        public async Task<bool> AddUser(UserEntity user)
        {
            try
            {
                string sql = """
                    INSERT INTO mrp_user (username, password) 
                    VALUES (@name, @pwd)
                    """;
                var sqlParams=new Dictionary<string, object?> { 
                    ["name"] = user.Username, 
                    ["pwd"] = user.Password 
                };
                var changedRows = await _postgres.SQLWithoutReturns(sql, sqlParams);

                return changedRows > 0;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteUser(string username)
        {
            try
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
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdatePassword(UserEntity updated)
        {
            try
            {
                string sql = """
                    UPDATE mrp_user 
                    SET password = @password
                    WHERE username = @username
                    """;
                var sqlParams = new Dictionary<string, object?>
                {
                    ["username"] = updated.Username,
                    ["password"] = updated.Password
                };
                int changedRow = await _postgres.SQLWithoutReturns(sql, sqlParams);
                return changedRow > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}