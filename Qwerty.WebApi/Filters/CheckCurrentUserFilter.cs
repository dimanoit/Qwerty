using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Qwerty.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
