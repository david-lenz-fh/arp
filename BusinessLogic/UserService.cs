using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using DataAccess.Entities;
using BusinessLogic.Models;

namespace BusinessLogic
{
    public class UserService:IUserService
    {
        private IDAL _dal;
        private readonly byte[] secretPrivateKey = Encoding.UTF8.GetBytes("Tokendergeheimist");
        public UserService(IDAL dal)
        {
            _dal = dal;
        }
        public async Task<Token?> Login(User credentials)
        {
            string hashed=HashPassword(credentials.Password);
            UserEntity? loginUser=await _dal.UserRepo.FindUserByName(credentials.Username);
            if (loginUser == null || hashed != loginUser.Password)
            {
                return null;
            }
            return new Token(AES.Encrypt(credentials.Username, secretPrivateKey));
        }
        public async Task<Token?> Register(User credentials)
        {
            string hashed = HashPassword(credentials.Password);
            bool added = await _dal.UserRepo.AddUser(new UserEntity(credentials.Username, hashed));
            if (added)
            {
                return new Token(AES.Encrypt(credentials.Username, secretPrivateKey));
            }
            return null;
        }
        public async Task<User?> GetUserByToken(string token)
        {
            string plaintext=AES.Decrypt(token, secretPrivateKey);
            string[] tokenVariables = plaintext.Split(":");
            if (tokenVariables.Length != 2 && tokenVariables[0]!=null && tokenVariables[1]!=null)
            {
                return null;
            }
            long expireTSTMP=0;
            try
            {
                expireTSTMP = long.Parse(tokenVariables[1]);
            }
            catch (Exception)
            {
                return null;
            }
            if (expireTSTMP < DateTime.Now.Ticks)
            {
                return null;
            }
            
            UserEntity? found = await _dal.UserRepo.FindUserByName(tokenVariables[0]);
            if(found == null)
            {
                return null;
            }
            return new User(found.Username, found.Password);
            
        }
        private static string HashPassword(string rawData)
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
