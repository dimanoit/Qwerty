using Microsoft.AspNet.Identity.Owin;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Qwerty.WEB.Controllers
{

    [Authorize(Roles = "admin")]
    [RoutePrefix("api/admin")]
    public class AdminController : ApiController
    {
        private IAdminService _adminService;
        public IUserService UserService => Request.GetOwinContext().GetUserManager<IUserService>();
        private async Task<UserDTO> GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            var UserName = IdentityClaims.FindFirst("sub").Value;
            return await UserService.FindUserByUsername(UserName);
        }

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPut]
        [Route("block/{UserId}")]
        public async Task<IHttpActionResult> BlockUser([FromUri]string UserId)
        {

            if (UserId != null && UserId != "")
            {
                OperationDetails operationDetails = await _adminService.BlockUserAsync(UserId);
                return Ok(operationDetails);
            }
            else return BadRequest("Cant find user with this id " + UserId);
        }

        [HttpPut]
        [Route("unblock/{UserId}")]
        public async Task<IHttpActionResult> UnblockUser([FromUri]string UserId)
        {

            if (UserId != null && UserId != "")
            {
                OperationDetails operationDetails = await _adminService.UnblockUserAsync(UserId);
                return Ok(operationDetails);
            }
            else return BadRequest("Cant find user with this id " + UserId);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllUserWithBlockedStatus()
        {

            IEnumerable<UserDTO> users = await UserService.GetUsers();
            if (users == null) return NotFound();
            else
            {
                var user = await GetCurrentUser();
                return Ok(users.Where(x => x.Id != user.Id));
            }
        }

    }
}
