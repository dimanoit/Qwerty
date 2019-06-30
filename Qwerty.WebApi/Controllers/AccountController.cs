using System.Collections.Generic;
using System.Threading.Tasks;
using Qwerty.WEB.Models;
using Qwerty.BLL.Interfaces;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using System.Security.Claims;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Qwerty.WEB.Controllers
{
    [Authorize(Roles = "user", AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IUserService UserService;
        private async Task<UserDTO> GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity) User.Identity;
            return await UserService.FindUserByIdAsync(IdentityClaims.Name);
        }

        public AccountController(IUserService userService)
        {
            UserService = userService;
        }

        [AllowAnonymous]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody]RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                UserDTO userDto = new UserDTO
                {
                    Password = model.Password,
                    UserName = model.UserName,
                    Name = model.Name,
                    Surname = model.SurName,
                    Roles = new string[] { "user" }
                };
                OperationDetails operationDetails = await UserService.CreateUserAsync(userDto);
                return Ok(operationDetails);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception unpredicatableException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAccounts([FromQuery]FindUserViewModel findUser)
        {
            try
            {
                IEnumerable<UserDTO> users = await UserService
                    .GetUsers(findUser?.Name, findUser?.Surname, findUser?.Country, findUser?.City);
                if (users == null) throw new ValidationException("Users not found.", "");
                var user = await GetCurrentUser();
                return Ok(users.Where(x => x.Id != user.Id));
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

        [HttpDelete]
        [Route("{userId}")]
        public async Task<ActionResult> DeleteUser( string userId)
        {
            try
            {
                var user = await GetCurrentUser();
                if (user == null || user.Id != userId) throw new ValidationException("Current user not found", "");
                await UserService.DeleteUser(userId);
                return Ok();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpPost]
        [Route("{userId}/uploadImage")]
        public async Task<ActionResult> UploadImage( string userId)
        {
            try
            {
                var user = await GetCurrentUser();
                if (user == null || user.Id != userId) throw new ValidationException("Current user not found", "");
                var postedFile = Request.Form.Files["Image"];
                if (postedFile == null) throw new ValidationException("File has not been attached", "");
                if ((postedFile.ContentType.Contains("jpg") || postedFile.ContentType.Contains("png") || postedFile.ContentType.Contains("jpeg")) == false)
                {
                    throw new ValidationException("The file has the wrong format.", postedFile.ContentType);
                }
                var ImageUrl = "C:/Users/Dima/Documents/Programming/Angular/QwertyAngular/src/assets/ProfileImages/" + postedFile.FileName;
                using (var stream = new FileStream(ImageUrl, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                OperationDetails operationDetails = await UserService.UploadImage(ImageUrl, user.UserName);
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
        public async Task<ActionResult> ChangeUser([FromBody] UserProfileViewModel userModel)
        {
            try
            {
                if (ModelState.IsValid == false) return BadRequest(ModelState);
                UserDTO user = new UserDTO
                {
                    AboutUrl = userModel.AboutUrl,
                    City = userModel.City,
                    Country = userModel.Country,
                    Email = userModel.Email,
                    Name = userModel.Name,
                    Surname = userModel.Surname,
                    Phone = userModel.Phone,
                    Id = (await GetCurrentUser()).Id
                };
                OperationDetails operationDetails = await UserService.ChangeProfileInformation(user);
                if (operationDetails.Succedeed == false)
                {
                    throw new ValidationException(operationDetails.Message, operationDetails.Property);
                }
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
        [Route("{userId}")]
        public async Task<ActionResult> GetUser(string userId)
        {
            try
            {
                var user = await GetCurrentUser();
                if (user == null || user.Id != userId) throw new ValidationException("Current user not found", "");
                return Ok(user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}