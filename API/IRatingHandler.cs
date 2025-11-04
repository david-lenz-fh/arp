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
        public Task GetUserFavourites(HttpListenerContext ctx, Dictionary<string, string> parameters);
        public Task PostRating(HttpListenerContext ctx, Dictionary<string, string> parameters);
        public Task ConfirmComment(HttpListenerContext ctx, Dictionary<string, string> parameters);
        public Task DeleteRating(HttpListenerContext ctx, Dictionary<string, string> parameters);
        public Task PutRating(HttpListenerContext ctx, Dictionary<string, string> parameters);
        public Task Favourite(HttpListenerContext ctx, Dictionary<string, string> parameters);
        public Task Unfavourite(HttpListenerContext ctx, Dictionary<string, string> parameters);
    }
}
