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

        //Für die Zwischenabgabe zum Testen
        public Task GetUser(HttpListenerContext ctx);
    }
}
