using System;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Qwerty.BLL.Infrastructure;

namespace Qwerty.WebApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
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
                if (context.Response.HasStarted)
                {
                    throw;
                }

                var handlingResponse = HandleException(ex);

                if (handlingResponse != null)
                {
                    await WriteResponseAsync(context, handlingResponse);
                }
            }
        }

        private ErrorDetails HandleException(Exception ex)
        {
            switch (ex)
            {
                case AuthenticationException authEx:
                    _logger.LogWarning(authEx, authEx.Message);
                    return new ErrorDetails { Message = ex.Message, StatusCode = (int)HttpStatusCode.Unauthorized };

                case ValidationException badRequestEx:
                    _logger.LogWarning(badRequestEx, badRequestEx.Message);
                    return new ErrorDetails { Message = ex.Message, StatusCode = (int)HttpStatusCode.BadRequest };

                case TaskCanceledException _:
                case OperationCanceledException _:
                    _logger.LogInformation("Request has been cancelled");
                    return null;
                case BadHttpRequestException kestrelEx:
                    _logger.LogWarning(kestrelEx, kestrelEx.Message);
                    return new ErrorDetails { Message = ex.Message, StatusCode = (int)HttpStatusCode.RequestEntityTooLarge };
                default:
                    _logger.LogError(ex, ex.Message);
                    return new ErrorDetails { Message = "Something wrong.", StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        private Task WriteResponseAsync(HttpContext context, ErrorDetails details)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = details.StatusCode;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(details));
        }

        
        private class ErrorDetails
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }
        
    }
}