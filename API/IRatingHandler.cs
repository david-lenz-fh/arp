using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public interface IRatingHandler
    {
        public Task GetUserRatings(HttpListenerContext ctx, Dictionary<string, string> parameters);
    }
}
