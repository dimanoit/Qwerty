using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Qwerty.WebApi.Filters
{
    public class CheckCurrentUserFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestUserId = context.ActionArguments["userId"].ToString();
            var identityName = context.HttpContext.User.Identity.Name;
            if (identityName != requestUserId) throw new ArgumentException("Current user not found");
        }
    }
}
