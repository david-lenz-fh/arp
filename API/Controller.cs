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
        
        public static void WriteJson<T>(HttpListenerContext ctx, T toJSON)
        {
            string json = JsonSerializer.Serialize(toJSON);
            
            ctx.Response.ContentEncoding = System.Text.Encoding.UTF8;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);

            ctx.Response.ContentType = "application/json";
            ctx.Response.ContentLength64 = buffer.Length; 
            ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
            ctx.Response.Close();
        }
        public static Token? ReadBearerToken(HttpListenerContext ctx)
        {
            string token=ctx.Request.Headers["Authorization"]??"";
            int overheadSize = "Bearer ".Length;
            if (token.Length <= overheadSize)
            {
                return null;
            }
            token = token.Substring(overheadSize);
            return new Token(token);
        }
        public async static Task<T?> ReadJSONRequestAsync<T>(HttpListenerContext ctx)
        {
            using var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding);
            var body = await reader.ReadToEndAsync();

            return JsonSerializer.Deserialize<T>(body);
        }
        public static void SendEmptyStatus(HttpListenerContext ctx, HttpStatusCode statusCode, string description)
        {
            ctx.Response.StatusCode = (int)statusCode;
            ctx.Response.StatusDescription = description;
            ctx.Response.Close();

        }
    }
}
