using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IUserRepository
    {

        public Task<UserEntity?> FindUserByName(string username);
        public Task<bool> AddUser(UserEntity user);
        public Task<bool> DeleteUser(string username);
        public Task<bool> UpdateUser(UserEntity updated);
        public Task<bool> AddActivityPoints(string username, int points);
        public Task<List<UserActivity>> GetLeaderboard(int topXusers);
    }
}
