using Microsoft.AspNetCore.Builder;

namespace Qwerty.WebApi.Configurations
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
