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
            var login = await ReadJSONRequestAsync<User>(ctx);
            if (login == null)
            {
                SendEmptyStatus(ctx, 400);
                return;
            }            
            Token? token = await _bl.UserService.Login(login);
            if (token == null)
            {
                SendEmptyStatus(ctx, 404);
                return;
            }
            WriteJson<Token>(ctx, token);
        }
        public async Task GetUser(HttpListenerContext ctx)
        {
            var token = await ReadJSONRequestAsync<Token>(ctx);
            if(token == null)
            {
                SendEmptyStatus(ctx, 400);
                return;
            }
            User? added=await _bl.UserService.GetUserByToken(token.token);
            if (added == null)
            {
                SendEmptyStatus(ctx, 400);
                return;
            }
            WriteJson<User>(ctx, added);
        }
    }    
}
