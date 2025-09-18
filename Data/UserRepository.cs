using MRP.model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRP.Data
{
    internal class UserRepository
    {
        PostgresDB _postgres;
        public UserRepository(PostgresDB postgres)
        {
            _postgres = postgres;
        }

        public async Task<User?> FindById(int Id)
        {
            var reader = await _postgres.SQLWithReturns("SELECT user_id, username, password FROM mrp_user WHERE user_id=@id", new Dictionary<string, object> { ["@id"] = Id });
            if (await reader.ReadAsync())
            {
                return new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
            }
            return null;
        }
        public async Task<User?> AddUser(User user)
        {
            try
            {
                var reader = await _postgres.SQLWithReturns("INSERT INTO mrp_user (username, password) VALUES (@name, @pwd) RETURNING user_id",
                new Dictionary<string, object> { ["name"] = user.Username, ["pwd"] = user.Password });
                if (await reader.ReadAsync())
                {
                    return user with { Id = reader.GetInt32(0) };
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
                int changedRow=await _postgres.SQLWithoutReturns("DELETE FROM mrp_user WHERE user_id=@id", new Dictionary<string, object> { ["id"] = Id });
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
    }
}