
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shamdev.TOA.Web.Infrastructure.TypeOfResultQuery;
using System;
using System.Threading.Tasks;
using BLL.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Shamdev.TOA.Web
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger(nameof(ErrorHandlingMiddleware));
        }

        public async Task Invoke(HttpContext ctx)
        {
            try
            {
                await _next.Invoke(ctx);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ctx, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            var errorCode = "0"; //Получаете в зависимости от типа Exception
            var statusCode = 400;
            var message = exception.Message;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(ErrorResponse.Build(errorCode, message));
        }
    }


    public class ErrorResponse
    {
        public static string Build(string code, string message)
        {
            JsonSerializerSettings o = new JsonSerializerSettings();
            o.NullValueHandling = NullValueHandling.Ignore;
            o.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            o.Converters.Add(new StringEnumConverter());
            o.Converters.Add(new JsonInt32Converter());
            o.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(new FailResultQuery() { Message = message },o);
        }
    }
}
