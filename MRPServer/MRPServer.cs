using API;
using BusinessLogic;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace mrp
{
    internal class MRPServer
    {
        private readonly IAPI api;
        private readonly HttpListener listener = new HttpListener();
        private readonly Dictionary<(string,string), Action<HttpListenerContext>> routes;
        private readonly string url;
        private int retry = 3;

        internal MRPServer(IAPI api)
        {       
            url = "http://localhost:8080/";
            listener.Prefixes.Add(url);

            routes = new Dictionary<(string path, string httpMethod), Action<HttpListenerContext>>
            {
                [("/users/register", "POST")] = ctx => WriteResponse(ctx, "Meine Anime Rating Platform Startseite"),
                [("/users/login", "POST")] = ctx => api.UserHandler.Login(ctx),
                [("/users/{userId}/profile","GET")] = ctx => WriteResponse(ctx, "Test!"),
                [("/users/{userId}/profile", "PUT")] = ctx => WriteResponse(ctx, "Test!"),
                [("/users/{userdId}/ratings","GET")] = ctx => WriteResponse(ctx, "Test!"),
                [("/users/{userId}/profiles","GET")] = ctx => WriteResponse(ctx, "Test!"),
                [("/media","GET")] = ctx => WriteResponse(ctx, "Test!"),
                [("/media/{mediaId}", "DELETE")] = ctx => WriteResponse(ctx, "Test!"),
                [("/media/{mediaId}", "GET")] = ctx => WriteResponse(ctx, "Test!"),
                [("/media/{mediaId}", "PUT")] = ctx => WriteResponse(ctx, "Test!"),
                [("/media/{mediaId}/rate","POST")] = ctx => WriteResponse(ctx, "Test!"),
                [("/ratings/{ratingId}/like","POST")] = ctx => WriteResponse(ctx, "Test!"),
                [("/ratings/{ratingId}","PUT")] = ctx => WriteResponse(ctx, "Test!"),
                [("/ratings/{ratingId}", "DELETE")] = ctx => WriteResponse(ctx, "Test!"),
                [("/ratings/{ratingId}/confirm", "POST")] = ctx => WriteResponse(ctx, "Test!"),
                [("/media/{mediaId}/favorite","POST")] = ctx => WriteResponse(ctx, "Test!"),
                [("/media/{mediaId}/favorite", "DELETE")] = ctx => WriteResponse(ctx, "Test!"),
                [("/users/{userId}/recommendations","GET")] = ctx => WriteResponse(ctx, "Test!"),
                [("/leaderboard","GET")] = ctx => WriteResponse(ctx, "Test!")
            };
        }
        public void Listen()
        {
            try
            {
                listener.Start();
                Console.WriteLine(String.Format("Server gestartet auf {0}", url));

                while (true)
                {
                    HttpListenerContext requestContext = listener.GetContext();
                    _ = Task.Run(() => HandleRequest(requestContext));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
            finally
            {
                if (retry > 0)
                {
                    retry--;
                    Console.WriteLine("Trying to restart server (Try {0})", 3 - retry);
                    Listen();
                }
                else
                {
                    Console.WriteLine("Server terminated");
                }
            }
        }

        private void HandleRequest(HttpListenerContext requestContext)
        {
            string path = requestContext.Request.Url?.AbsolutePath ?? "";
            string httpMethod = requestContext.Request.HttpMethod;

            if (routes.TryGetValue((path, httpMethod), out var handler))
            {
                handler(requestContext);
            }
            else
            {
                WriteResponse(requestContext, "404 Not Found");
            }
        }

        //SRP (keine Handler von Requests in der selben Klasse wie Server starten)
        private void WriteResponse(HttpListenerContext ctx, string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            ctx.Response.ContentLength64 = buffer.Length;
            ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
            ctx.Response.OutputStream.Close();
        }
        private void Error404(HttpListenerContext ctx)
        {
            byte[] buffer = Encoding.UTF8.GetBytes("Not Found");
            ctx.Response.ContentLength64 = buffer.Length;
            ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
            ctx.Response.OutputStream.Close();
            ctx.Response.StatusCode = 404;
        }
    }
}
