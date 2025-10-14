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
                SendEmptyStatus(ctx, 400, "No Login Information was send");
                return;
            }            
            Token? token = await _bl.UserService.Login(login);
            if (token == null)
            {
                SendEmptyStatus(ctx, 404,"Couldn't authenticate");
                return;
            }
            WriteJson<Token>(ctx, token);
        }
        public async Task Register(HttpListenerContext ctx)
        {
                
        }
        public async Task GetUser(HttpListenerContext ctx)
        {
            var token = await ReadJSONRequestAsync<Token>(ctx);
            if(token == null)
            {
                SendEmptyStatus(ctx, 400, "No Token was send");
                return;
            }
            User? added=await _bl.UserService.GetUserByToken(token.token);
            if (added == null)
            {
                SendEmptyStatus(ctx, 400,"Token invalid");
                return;
            }
            WriteJson<User>(ctx, added);
        }
    }    
}
