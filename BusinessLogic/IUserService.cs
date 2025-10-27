using BusinessLogic.Models;
using System.Net;

namespace BusinessLogic
{
    public interface IUserService
    {
        public Task<Token?> Login(Login credentials);
        public Task<Token?> Register(Login credentials);
        public Task<User?> GetUserByToken(string token);
        public Task<User?> FindUserByName(string username);
        public Task<bool> UpdateUser(User updatedUser);
    }
}