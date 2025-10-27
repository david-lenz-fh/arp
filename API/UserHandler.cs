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
            Token? token = await _bl.UserService.Login(new Login(login.Username, login.Password));
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized,"Couldn't authenticate");
                return;
            }
            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
            ctx.Response.StatusDescription = "Login successfull";
            WriteJson<Token>(ctx, token);
        }
        public async Task Register(HttpListenerContext ctx)
        {
            var registrationData= await ReadJSONRequestAsync<LoginDTO>(ctx);
            if (registrationData == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No User Information");
                return;
            }
            Token? token=await _bl.UserService.Register(new Login(registrationData.Username, registrationData.Password));
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.InternalServerError, "Error creating Account");
                return;
            }
            ctx.Response.StatusCode = (int)HttpStatusCode.Created;
            ctx.Response.StatusDescription = "User Registered";
            WriteJson<Token>(ctx, token);
        }
        

        public async Task GetUserProfile(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? username = parameters["userId"];
            if(username == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No UserId");
                return;
            }
            username = HttpUtility.UrlDecode(username);
            User? foundUser = await _bl.UserService.FindUserByName(username);
            if(foundUser == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, String.Format("User \"{0}\" not found", username));
                return;
            }
            var profile = new ProfileDTO(foundUser.Email, foundUser.FavouriteGenre);
            WriteJson<ProfileDTO>(ctx, profile);
        }
        public async Task UpdateUserProfile(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            User? authenticatedUser = await _bl.UserService.GetUserFromToken(ReadBearerToken(ctx));
            if (authenticatedUser == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized, "Authentication failed");
                return;
            }
            ProfileDTO? updateProfile = await ReadJSONRequestAsync<ProfileDTO>(ctx);
            if (updateProfile == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Profile Data in Request Body");
                return;
            }
            string? username = parameters["userId"];
            if (username == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No username");
                return;
            }
            username = HttpUtility.UrlDecode(username);
            User? foundUser = await _bl.UserService.FindUserByName(username);
            if (foundUser == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, String.Format("User \"{0}\" not found", username));
                return;
            }
            if (foundUser.Username != authenticatedUser.Username)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized, "You have not authorization to change this profile");
                return;
            }
            var updateUser = new User(foundUser.Username, foundUser.Password, updateProfile.Email, updateProfile.FavouriteGenre);
            bool wasUpdated = await _bl.UserService.UpdateUser(updateUser);
            if (!wasUpdated) 
            {
                SendEmptyStatus(ctx, HttpStatusCode.InternalServerError, "Couldnt update User Profile");
                return;
            }
            SendEmptyStatus(ctx, HttpStatusCode.OK, "User profile was successfully updated");
        }
    }    
}
