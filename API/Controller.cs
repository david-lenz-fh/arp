using BusinessLogic;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API
{
    public class Controller
    {
        protected IBL _bl;
        public Controller(IBL bl) {
            _bl = bl; 
        }
        
        public async static Task WriteJSONResponseAsync<T>(HttpListenerContext ctx, T toJSON)
        {
            string json = JsonSerializer.Serialize(toJSON);

            ctx.Response.ContentType = "application/json";
            ctx.Response.ContentEncoding = System.Text.Encoding.UTF8;

            using var writer = new StreamWriter(ctx.Response.OutputStream);
            await writer.WriteAsync(json);
            ctx.Response.Close();
        }

        public async static Task<T?> ReadJSONRequestAsync<T>(HttpListenerContext ctx)
        {
            using var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding);
            var body = await reader.ReadToEndAsync();

            return JsonSerializer.Deserialize<T>(body);
        }
        public static void SendEmptyStatus(HttpListenerContext ctx, int statusCode)
        {
            ctx.Response.StatusCode = statusCode;
            ctx.Response.Close();

        }
    }
}
