using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public interface IMediaHandler
    {
        public Task GetMedia(HttpListenerContext ctx);
        public Task FindMediaById(HttpListenerContext ctx, Dictionary<string, string> parameters);
        public Task DeleteMediaById(HttpListenerContext ctx, Dictionary<string,string> parameters);
        public Task PostMedia(HttpListenerContext ctx);
    }
}
