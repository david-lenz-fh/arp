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
        public async Task<Token?> Login(Login credentials)
        {
            string hashed=Hash(credentials.Password);
            UserEntity? loginUser=await _dal.UserRepo.FindUserByName(credentials.Username);
            if (loginUser == null || hashed != loginUser.Password)
            {
                return null;
            }
            string toEncrypt = GetValidTimeStamp() + ":" + credentials.Username;
            return new Token(AES.Encrypt(toEncrypt, secretPrivateKey));
        }
        public async Task<User?> GetUserByToken(string token)
        {
            string plaintext = "";
            try
            {
                plaintext = AES.Decrypt(token, secretPrivateKey);
            }
            catch (Exception)
            {
                return null;
            }
            string[] tokenVariables = plaintext.Split(":");
            if (tokenVariables.Length != 2 || tokenVariables[0] == null || tokenVariables[1] == null )
            {
                return null;
            }
            long expireTSTMP = 0;
            try
            {
                expireTSTMP = long.Parse(tokenVariables[0]);
            }
            catch (Exception)
            {
                return null;
            }
            if (expireTSTMP < DateTime.Now.Ticks)
            {
                return null;
            }
            return await FindUserByName(tokenVariables[1]);
            

        }
        public async Task<User?> FindUserByName(string username)
        {
            UserEntity? found = await _dal.UserRepo.FindUserByName(username);
            if (found == null)
            {
                return null;
            }
            return new User(found.Username, found.Password, found.Email, found.FavouriteGenre);
        }

        public async Task<Token?> Register(Login credentials)
        {
            string hashed = Hash(credentials.Password);
            bool added = await _dal.UserRepo.AddUser(new UserEntity(credentials.Username, hashed, null, null));
            if (added)
            {
                string toEncrypt = GetValidTimeStamp() + ":" + credentials.Username;
                return new Token(AES.Encrypt(toEncrypt, secretPrivateKey));
            }
            return null;
        }

        public async Task<bool> UpdateUser(User updatedUser)
        {
            var update=new UserEntity(updatedUser.Username, updatedUser.Password, updatedUser.Email, updatedUser.FavouriteGenre);
            return await _dal.UserRepo.UpdateUser(update);
        }
        public async Task<User?> GetUserFromToken(Token? token)
        {
            if (token == null)
            {
                return null;
            }
            User? found = await GetUserByToken(token.token);
            if (found == null)
            {
                return null;
            }
            return found;
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
