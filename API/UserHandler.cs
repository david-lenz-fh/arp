using BusinessLogic;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
        public async Task GetUser(HttpListenerContext ctx)
        {
            var token = ReadBearerToken(ctx);
            if(token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized, "No Token was send");
                return;
            }
            User? added=await _bl.UserService.GetUserByToken(token.token);
            if (added == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized,"Token invalid");
                return;
            }
            WriteJson<User>(ctx, added);
        }

        public Task GetUserProfile(HttpListenerContext ctx, Dictionary<string, string>? parameters)
        {
            return null;
        }
    }    
}
