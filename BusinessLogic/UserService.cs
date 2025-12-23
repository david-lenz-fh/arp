using BusinessLogic.Models;
using DataAccess;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class UserService:IUserService
    {
        private IDAL _dal;
        
        private readonly byte[] secretPrivateKey;

        public UserService(IDAL dal)
        {
            _dal = dal;
            using (SHA256 sha256 = SHA256.Create())
            {
                secretPrivateKey=sha256.ComputeHash(Encoding.UTF8.GetBytes("MeinGeheimToken"));
            }
        }
        public async Task<Result<string>> Login(Login credentials)
        {
            string hashed=Hash(credentials.Password);
            UserEntity? loginUser=await _dal.UserRepo.FindUserByName(credentials.Username);
            if (loginUser == null || hashed != loginUser.Password)
            {
                return new Result<string>(null, new ResultResponse(BL_Response.AuthenticationFailed,"Couldnt authenticate"));
            }
            string toEncrypt = GetValidTimeStamp() + ":" + credentials.Username;
            try
            {
                string token = AES.Encrypt(toEncrypt, secretPrivateKey);
                return new Result<string>(token, new ResultResponse(BL_Response.OK, "Logged in successfully"));
            }
            catch (Exception) {
                return new Result<string>(null, new ResultResponse(BL_Response.InternalError,null));
            }
        }
        public async Task<Result<User>> AuthenticateUserByToken(string authenticationToken)
        {
            string plaintext = "";
            try
            {
                plaintext = AES.Decrypt(authenticationToken, secretPrivateKey);
            }
            catch (Exception)
            {
                return new Result<User>(null, new ResultResponse(BL_Response.InternalError, null));
            }
            string[] tokenVariables = plaintext.Split(":");
            if (tokenVariables.Length != 2 || tokenVariables[0] == null || tokenVariables[1] == null )
            {
                return new Result<User>(null, new ResultResponse(BL_Response.Unauthorized, "Token is malformed"));
            }
            long expireTSTMP = 0;
            try
            {
                expireTSTMP = long.Parse(tokenVariables[0]);
            }
            catch (Exception)
            {
                return new Result<User>(null, new ResultResponse(BL_Response.Unauthorized, "Couldnt parse timestamp of token"));
            }
            if (expireTSTMP < DateTime.Now.Ticks)
            {
               return new Result<User>(null, new ResultResponse(BL_Response.Unauthorized, "Token is expired"));
            }
            var returnValue = await FindUserByName(tokenVariables[1]);
            if (returnValue.Value == null)
            {
                return new Result<User>(null, returnValue.Response);
            }
            return new Result<User>(returnValue.Value, new ResultResponse(BL_Response.OK, "User found"));
        }
        public async Task<Result<User>> FindUserByName(string username)
        {
            UserEntity? found = await _dal.UserRepo.FindUserByName(username);
            if (found == null)
            {
                return new Result<User>(null, new ResultResponse(BL_Response.NotFound, "user not found"));
            }
            var returnValue= new User(found.Username, found.Password, found.Email, found.FavouriteGenre);
            return new Result<User>(returnValue, new ResultResponse(BL_Response.OK, "User found"));
        }

        public async Task<Result<string>> Register(Login credentials)
        {
            string hashed = Hash(credentials.Password);
            bool added = await _dal.UserRepo.AddUser(new UserEntity(credentials.Username, hashed, null, null));
            if (!added)
            {
                return new Result<string>(null, new ResultResponse(BL_Response.InternalError, "Couldnt register user"));
            }
            string toEncrypt = GetValidTimeStamp() + ":" + credentials.Username;
            try
            {
                string token = AES.Encrypt(toEncrypt, secretPrivateKey);
                return new Result<string>(token, new ResultResponse(BL_Response.OK, "Registered"));
            }
            catch (Exception)
            {
                return new Result<string>(null, new ResultResponse(BL_Response.InternalError, "Couldnt register user"));
            }
        }

        public async Task<ResultResponse> UpdateProfile(string authenticationToken, Profile updatedProfile)
        {
            var user = await AuthenticateUserByToken(authenticationToken);
            if (user.Value == null)
            {
                return user.Response;
            }
            var update=new UserEntity(user.Value.Username, user.Value.Password, updatedProfile.Email, updatedProfile.FavouriteGenre);
            if(!await _dal.UserRepo.UpdateUser(update))
            {
                return new ResultResponse(BL_Response.InternalError, "Couldnt update user");
            }
            return new ResultResponse(BL_Response.OK, "User was updated");
        }
        public async Task<Result<List<UserRank>>> Leaderboard()
        {
            int topX = 10;
            var re=new List<UserRank>();
            var leaderboard = await _dal.UserRepo.GetLeaderboard(topX);
            int placement = 1;
            foreach (var rank in leaderboard)
            {
                re.Add(new UserRank(placement, rank.ActivityPoints, rank.Username));
                placement++;
            }
            return new Result<List<UserRank>>(re, new ResultResponse(BL_Response.OK, null));
        }
        private static long GetValidTimeStamp()
        {
            return DateTime.Now.Add(TimeSpan.FromSeconds(7200)).Ticks;
        }
        
        private static string Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData) ?? new byte[] { });

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); 
                }
                return builder.ToString();
            }
        }

    }
}
