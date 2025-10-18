using BusinessLogic.Models;

namespace BusinessLogic
{
    public interface IUserService
    {
        public Task<Token?> Login(Login credentials);
        public Task<Token?> Register(Login credentials);
        public Task<User?> GetUserByToken(string token);

    }
}