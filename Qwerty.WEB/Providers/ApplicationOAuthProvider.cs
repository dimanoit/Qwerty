using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Qwerty.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Qwerty.WEB.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userService = context.OwinContext.GetUserManager<IUserService>();
            var user = await userService.FindUser(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            var userRoles = userService.GetRolesByUserId(user.Id);
            foreach (string roleName in userRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
            }
            AuthenticationProperties properties = CreateProperties(user.Id,userRoles);
            AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);
            context.Validated(ticket);
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }
        private  AuthenticationProperties CreateProperties(string id, IList<string> userRoles)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "Id", id },
                { "roles", Newtonsoft.Json.JsonConvert.SerializeObject(userRoles) }
            };
            return new AuthenticationProperties(data);
        }
    }
}