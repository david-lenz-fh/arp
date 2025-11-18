using BusinessLogic.Models;
using System.Net;

namespace BusinessLogic
{
    public interface IUserService
    {
        public Task<Result<string>> Login(Login credentials);
        public Task<Result<string>> Register(Login credentials);
        public Task<Result<User>> AuthenticateUserByToken(string authenticationToken);
        public Task<Result<User>> FindUserByName(string username);
        public Task<ResultResponse> UpdateProfile(string authenticationToken, Profile updatedProfile);
        public Task<Result<List<UserRank>>> Leaderboard();
    }
}