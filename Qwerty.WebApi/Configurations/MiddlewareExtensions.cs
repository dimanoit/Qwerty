using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qwerty.WebApi.Configurations
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
