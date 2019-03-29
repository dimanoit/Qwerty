using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Interfaces;
using Qwerty.WEB.Models;

namespace UIWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        public IUserService UserService => Request.GetOwinContext().GetUserManager<IUserService>();
        private IAuthenticationManager AuthenticationManager => Request.GetOwinContext().Authentication;
        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserDTO userDto = new UserDTO
            {
                Password = model.Password,
                UserName = model.UserName,
                Role = "user"
            };
            OperationDetails operationDetails = await UserService.Create(userDto);
            if (!operationDetails.Succedeed)
                return GetErrorResult(operationDetails);
            return Ok();
        }
        private IHttpActionResult GetErrorResult(OperationDetails result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succedeed)
            {
                ModelState.AddModelError(result.Property, result.Message);

                if (ModelState.IsValid)
                {
                    return BadRequest();
                }
                return BadRequest(ModelState);
            }

            return null;
        }
    }
}