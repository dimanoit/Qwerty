using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Qwerty.WebApi.Configurations
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            if (exception is ArgumentException)
            {
                code = HttpStatusCode.BadRequest;

                Log.Error(exception, $"Exception was in {context.Request.Path}");
            }
            else if (exception is AuthenticationException)
            {
                code = HttpStatusCode.Unauthorized;
                Log.Error(exception, $"Exception was in {context.Request.Path}");
            }
            else if (exception is NotImplementedException)
            {
                code = HttpStatusCode.NotImplemented;
                Log.Error(exception, $"Exception was in {context.Request.Path}");
            }

            var result = JsonConvert.SerializeObject("Internal server error (^-^)");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            Log.Error(exception, $"Exception was in {context.Request.Path}");
            return context.Response.WriteAsync(result);
        }
    }
}
