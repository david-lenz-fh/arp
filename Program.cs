using System.Net;
using System.Text;

var arp = new ARPServer();
arp.Start();
public class ARPServer
{
    private readonly HttpListener listener = new HttpListener();
    private readonly Dictionary<string, Action<HttpListenerContext>> routes; 
    private readonly string url;

    public ARPServer()
    {
        url = "http://localhost:8080/";
        listener.Prefixes.Add(url);

        routes = new Dictionary<string, Action<HttpListenerContext>>
        {
            ["/"] = ctx => WriteResponse(ctx, "Meine Anime Rating Platform Startseite"),
            ["/test"] = ctx => WriteResponse(ctx, "Test!")
        };
    }
    public void Start()
    {
        listener.Start();
        Console.WriteLine(String.Format("Server läuft auf {0}", url));

        while (true)
        {
            HttpListenerContext requestContext = listener.GetContext();
            _ = Task.Run(() => HandleRequest(requestContext));
        }
    }

    private void HandleRequest(HttpListenerContext requestContext)
    {
        string path = requestContext.Request.Url?.AbsolutePath ?? "";

        if (routes.TryGetValue(path, out var handler))
        {
            handler(requestContext);
        }
        else
        {
            WriteResponse(requestContext, "404 Not Found");
        }
    }

    private void WriteResponse(HttpListenerContext ctx, string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        ctx.Response.ContentLength64 = buffer.Length;
        ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
        ctx.Response.OutputStream.Close();
    }

}