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
        private readonly RoutingNode routes;
        private readonly string url;

        internal MRPServer(IAPI api)
        {
            this.api = api;
            url = "http://localhost:8080/";
            listener.Prefixes.Add(url);

            routes = new RoutingNode(null, new Dictionary<string, RoutingNode>
            {
                ["users"] = new RoutingNode(null, new Dictionary<string, RoutingNode>
                {
                    ["register"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                    {
                        ["POST"] = (ctx, parameters) => api.UserHandler.Register(ctx)
                    }, null),
                    ["login"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                    {
                        ["POST"] = (ctx, parameters) => api.UserHandler.Login(ctx)
                    }, null),
                    ["{userId}"] = new RoutingNode(null, new Dictionary<string, RoutingNode>
                    {
                        ["profile"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                        {
                            ["GET"] = (ctx, parameters) => api.UserHandler.GetUserProfile(ctx, parameters),
                            ["PUT"] = (ctx, parameters) => api.UserHandler.UpdateUserProfile(ctx, parameters)
                        }, null),
                        ["ratings"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                        {
                            ["GET"] = (ctx, parameters) => api.RatingHandler.GetUserRatings(ctx, parameters)
                        }, null),
                        ["favorites"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                        {
                            ["GET"] = (ctx, parameters) => api.RatingHandler.GetUserFavourites(ctx, parameters)
                        }, null),
                        ["recommendations"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                        {
                            ["GET"] = (ctx, parameters) => Controller.SendEmptyStatus(ctx, HttpStatusCode.NotImplemented, "")
                        }, null)
                    })
                }),
                ["media"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                {
                    ["GET"] = (ctx, parameters) => api.MediaHandler.GetMedia(ctx),
                    ["POST"] = (ctx, parameters) => api.MediaHandler.PostMedia(ctx),
                }, new Dictionary<string, RoutingNode>
                {
                    ["{mediaId}"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                    {
                        ["DELETE"] = (ctx, parameters) => api.MediaHandler.DeleteMediaById(ctx, parameters),
                        ["GET"] = (ctx, parameters) => api.MediaHandler.FindMediaById(ctx, parameters),
                        ["PUT"] = (ctx, parameters) => api.MediaHandler.PutMedia(ctx, parameters)
                    }, new Dictionary<string, RoutingNode>
                    {
                        ["rate"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                        {
                            ["POST"] = (ctx, parameters) => Controller.SendEmptyStatus(ctx, HttpStatusCode.NotImplemented, "")
                        }, null),
                        ["favorite"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                        {
                            ["POST"] = (ctx, parameters) => Controller.SendEmptyStatus(ctx, HttpStatusCode.NotImplemented, ""),
                            ["DELETE"] = (ctx, parameters) => Controller.SendEmptyStatus(ctx, HttpStatusCode.NotImplemented, "")
                        }, null)
                    })
                }),
                ["ratings"] = new RoutingNode(null, new Dictionary<string, RoutingNode>
                {
                    ["{ratingId}"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                    {
                        ["DELETE"] = (ctx, parameters) => Controller.SendEmptyStatus(ctx, HttpStatusCode.NotImplemented, ""),
                        ["PUT"] = (ctx, parameters) => Controller.SendEmptyStatus(ctx, HttpStatusCode.NotImplemented, "")
                    }, new Dictionary<string, RoutingNode>
                    {
                        ["like"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                        {
                            ["POST"] = (ctx, parameters) => Controller.SendEmptyStatus(ctx, HttpStatusCode.NotImplemented, "")
                        }, null),
                        ["confirm"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                        {
                            ["POST"] = (ctx, parameters) => Controller.SendEmptyStatus(ctx, HttpStatusCode.NotImplemented, "")
                        }, null),
                    })
                }),
                ["leaderboard"] = new RoutingNode(new Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>
                {
                    ["GET"] = (ctx, parameters) => Controller.SendEmptyStatus(ctx, HttpStatusCode.NotImplemented, "")
                }, null)
            });
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
                Console.WriteLine("Server terminated");
            }
        }

        private void HandleRequest(HttpListenerContext requestContext)
        {
            string httpMethod = requestContext.Request.HttpMethod;
            string[] paths = (requestContext.Request.Url?.AbsolutePath ?? "").Split('/');

            Dictionary<string,string> parameters=new Dictionary<string, string>();
            var currentRoute = routes;
            foreach (string path in paths.Skip(1))
            {
                var (routeNode, parameter) = currentRoute.NextRoute(path);
                if (routeNode == null)
                {
                    Controller.SendEmptyStatus(requestContext, HttpStatusCode.NotFound, "Route nicht gefunden");
                    return;
                }
                currentRoute = routeNode;
                if(parameter != null) 
                {
                    parameters[parameter.Value.Item1] = parameter.Value.Item2;
                }
            }
            if (currentRoute._routes == null)
            {
                Controller.SendEmptyStatus(requestContext, HttpStatusCode.NotFound, "Route nicht gefunden");
                return;
            }
            if(!currentRoute._routes.TryGetValue(httpMethod, out var action))
            {
                Controller.SendEmptyStatus(requestContext, HttpStatusCode.NotFound, "Falsche HTTP Methode für diese Route");
                return;
            }
            action(requestContext, parameters);
        }
    }
}
