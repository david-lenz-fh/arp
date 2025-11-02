using BusinessLogic;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

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
        public static string? ReadBearerToken(HttpListenerContext ctx)
        {
            string token=ctx.Request.Headers["Authorization"]??"";
            int overheadSize = "Bearer ".Length;
            if (token.Length <= overheadSize)
            {
                return null;
            }
            token = token.Substring(overheadSize);
            return token;
        }
        public async static Task<T?> ReadJSONRequestAsync<T>(HttpListenerContext ctx)
        {
            using var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding);
            var body = await reader.ReadToEndAsync();

            return JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public static void SendEmptyStatus(HttpListenerContext ctx, HttpStatusCode statusCode, string description)
        {
            ctx.Response.StatusCode = (int)statusCode;
            ctx.Response.StatusDescription = description;
            ctx.Response.Close();

        }
        public static Dictionary<string, string> GetQueryParams(HttpListenerContext ctx) {
            var re=new Dictionary<string, string>();
            string? query = ctx.Request.Url?.Query;
            if (query == null || query.Length < 1) {
                return re;
            }
            List<string> queryParamsString = query.Substring(1).Split('&').ToList();
            foreach (string param in queryParamsString)
            {
                string[] pairs = param.Split('=');
                if (pairs.Length == 2)
                {
                    pairs[0] = HttpUtility.UrlDecode(pairs[0]);
                    pairs[1] = HttpUtility.UrlDecode(pairs[1]);
                    re[pairs[0]] = pairs[1];
                }
            }
            return re;
        }
        public static void SendResultResponse(HttpListenerContext ctx, ResultResponse response)
        {
            HttpStatusCode httpCode;
            switch (response.ResponseCode)
            {
                case BL_Response.InternalError:
                case BL_Response.CorruptedData:
                    httpCode = HttpStatusCode.InternalServerError;
                    break;
                case BL_Response.NotFound:
                    httpCode = HttpStatusCode.NotFound;
                    break;
                case BL_Response.AuthenticationFailed:
                case BL_Response.Unauthorized:
                    httpCode = HttpStatusCode.Unauthorized;
                    break;
                case BL_Response.OK:
                    httpCode = HttpStatusCode.OK;
                    break;
                default:
                    httpCode = HttpStatusCode.InternalServerError;
                    break;
            }
            SendEmptyStatus(ctx, httpCode, response.Message??"");
        }
    }
}
