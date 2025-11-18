using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public interface IUserHandler
    {
        public Task Login(HttpListenerContext ctx);
        public Task Register(HttpListenerContext ctx);
        public Task GetUserProfile(HttpListenerContext ctx, Dictionary<string, string> parameters);
        public Task UpdateUserProfile(HttpListenerContext ctx, Dictionary<string, string> parameters);
        public Task GetLeaderboard(HttpListenerContext ctx, Dictionary<string, string> parameters);

    }
}
