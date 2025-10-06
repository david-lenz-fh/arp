using BusinessLogic.Models;

namespace BusinessLogic
{
    public interface IUserService
    {
        public Task<Token?> Login(User credentials);
        public Task<Token?> Register(User credentials);
        public Task<User?> GetUserByToken(string token);

    }
}