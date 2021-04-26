using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qwerty.DAL.Identity;
using Qwerty.WEB.Models;
using Qwerty.WebApi.Filters;
using Serilog;

namespace Qwerty.WEB.Controllers
{
    [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IAdminService _adminService;
        private IUserService _userService;

        public AdminController(IAdminService adminService, IUserService userService)
        {
            _adminService = adminService;
            _userService = userService;
        }

        [AllowAnonymous]
        [ModelValidationFilter]
        [HttpPost]
        public async Task CreateAdmin([FromBody] RegisterModel model)
        {
            UserDTO userDto = new UserDTO
            {
                Password = model.Password,
                UserName = model.UserName,
                Name = model.Name,
                Surname = model.SurName,
                Roles = new[] {QwertyRoles.Admin}
            };

            await _userService.CreateUserAsync(userDto);
        }

        [HttpPut]
        [Route("block/{UserId}")]
        public async Task<ActionResult> BlockUser(string userId)
        {
            await _adminService.BlockUserAsync(userId);
            return Ok();
        }

        [HttpPut]
        [Route("unblock/{UserId}")]
        public async Task<ActionResult> UnblockUser(string userId)
        {
            await _adminService.UnblockUserAsync(userId);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> GetAllUserWithBlockedStatus()
        {
            var user = await _userService.FindUserByIdAsync(HttpContext.User.Identity.Name);
            IEnumerable<UserDTO> users = await _userService.GetUsers();
            if (users == null)
            {
                Log.Warning("Social network without people");
                return BadRequest("Social network without people");
            }

            return Ok(users.Where(x => x.Id != user.Id));
        }
    }
}