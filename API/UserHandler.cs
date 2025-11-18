using BusinessLogic;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace API
{
    public class UserHandler:Controller, IUserHandler
    {
        public UserHandler(IBL businessLayer):base(businessLayer) {
        }
        public async Task Login(HttpListenerContext ctx)
        {
            var login = await ReadJSONRequestAsync<LoginDTO>(ctx);
            if (login == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Login Information was send");
                return;
            }            
            var token = await _bl.UserService.Login(new Login(login.Username, login.Password));
            if (token.Value == null)
            {
                SendResultResponse(ctx, token.Response);
                return;
            }
            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
            ctx.Response.StatusDescription = "Login successfull";
            WriteJson(ctx, new { Token = token.Value });
        }
        public async Task Register(HttpListenerContext ctx)
        {
            var registrationData= await ReadJSONRequestAsync<LoginDTO>(ctx);
            if (registrationData == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No User Information");
                return;
            }
            var token=await _bl.UserService.Register(new Login(registrationData.Username, registrationData.Password));
            if (token.Value == null)
            {
                SendResultResponse(ctx, token.Response);
                return;
            }
            ctx.Response.StatusCode = (int)HttpStatusCode.Created;
            ctx.Response.StatusDescription = "User Registered";
            WriteJson(ctx, new { Token = token.Value });
        }
        

        public async Task GetUserProfile(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? username = parameters.GetValueOrDefault("userId");
            if(username == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No UserId");
                return;
            }
            username = HttpUtility.UrlDecode(username);
            var foundUser = await _bl.UserService.FindUserByName(username);
            if(foundUser.Value == null)
            {
                SendResultResponse(ctx, foundUser.Response);
                return;
            }
            var profile = new ProfileDTO(foundUser.Value.Email, foundUser.Value.FavouriteGenre);
            WriteJson<ProfileDTO>(ctx, profile);
        }
        public async Task UpdateUserProfile(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized, "No Bearer Token");
                return;

            }
            ProfileDTO? updateProfile = await ReadJSONRequestAsync<ProfileDTO>(ctx);
            if (updateProfile == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Profile Data in Request Body");
                return;
            }
            string? username = parameters.GetValueOrDefault("userId");
            if (username == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No username");
                return;
            }
            SendResultResponse(ctx, await _bl.UserService.UpdateProfile(token, new Profile(updateProfile.Email, updateProfile.FavouriteGenre)));
        }

        public async Task GetLeaderboard(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            var leaderboard=await _bl.UserService.Leaderboard();
            if (leaderboard.Value == null)
            {
                SendResultResponse(ctx, leaderboard.Response);
                return;
            }
            var re = new List<UserRankDTO>();
            foreach(var userRank in leaderboard.Value)
            {
                re.Add(new UserRankDTO(userRank.Placement, userRank.Score, userRank.Username));
            }
            WriteJson(ctx, re);
        }
    }    
}
