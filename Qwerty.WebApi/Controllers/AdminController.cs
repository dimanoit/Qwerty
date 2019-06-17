using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qwerty.DAL.Entities;

namespace Qwerty.WEB.Controllers
{

    [Authorize(Roles="admin",AuthenticationSchemes = "Bearer")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IAdminService _adminService;
        public IUserService UserService;
        private UserDTO GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            return UserService.FindUserByUsername(IdentityClaims.Name);
        }

        public AdminController(IAdminService adminService, IUserService userService)
        {
            _adminService = adminService;
            UserService = userService;
        }

        [HttpPut]
        [Route("block/{UserId}")]
        public async Task<ActionResult> BlockUser(string UserId)
        {
            try
            {
                OperationDetails operationDetails = await _adminService.BlockUserAsync(UserId);
                return Ok(operationDetails);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPut]
        [Route("unblock/{UserId}")]
        public async Task<ActionResult> UnblockUser(string UserId)
        {

            try
            {
                OperationDetails operationDetails = await _adminService.UnblockUserAsync(UserId);
                return Ok(operationDetails);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllUserWithBlockedStatus()
        {
            try
            {
                var user = GetCurrentUser();
                if (user == null) throw new ValidationException("Can`t find admin account", "");
                IEnumerable<UserDTO> users = await UserService.GetUsers();
                if (users == null) throw new ValidationException("No users", "");
                return Ok(users.Where(x => x.Id != user.Id));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
