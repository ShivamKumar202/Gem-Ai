using Gem.COMMON.ResultModel;
using Microsoft.AspNetCore.Http;

namespace Gem.BLL.Common.Utility
{
    public static class SD
    {
        public static string GetClientIp(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out Microsoft.Extensions.Primitives.StringValues value))
            {
                var ip = value.FirstOrDefault();
                if (!string.IsNullOrEmpty(ip))
                {
                    return ip.Split(',')[0];
                }
            }
            return context.Connection.RemoteIpAddress?.ToString();
        }
        public static string GetFileExtensionFromUrl(string url) => Path.GetExtension(url)?.ToLowerInvariant() ?? string.Empty;

       
        public static bool IsRequestvalid<T>(T obj) => obj != null;
        public static ResModel CreateResponse(bool success, string message, int statusCode) => new()
        {
            Success = success,
            Message = message,
            StatusCode = statusCode
        };
        public static bool IsIdValid(int id) => id > 0;
        public static ResModel<T> CreateResponse<T>(T data, bool success, string message, int statusCode) => new()
        {
            Data = data,
            Success = success,
            Message = message,
            StatusCode = statusCode
        };

        
        public static ResListModel<T> CreateResListResponse<T>(ICollection<T> data, bool success, string message, int statusCode) => new()
        {
            Data = data,
            Success = success,
            Message = message,
            StatusCode = statusCode
        };
    }
}
