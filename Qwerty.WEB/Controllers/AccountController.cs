using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using Qwerty.WEB.Models;
using Qwerty.BLL.Interfaces;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using System.Security.Claims;
using AutoMapper;
using System.Linq;
using System;

namespace UIWebApi.Controllers
{
    [Authorize(Roles = "user")]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        public IUserService UserService => Request.GetOwinContext().GetUserManager<IUserService>();


        private UserDTO GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            var UserName = IdentityClaims.FindFirst("sub").Value;
            return UserService.FindUserByUsername(UserName);
        }

        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register([FromBody]RegisterModel model)
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
                if (!operationDetails.Succedeed) throw new ValidationException(operationDetails.Message, operationDetails.Property);
                return Ok(operationDetails);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllAccounts([FromUri] FindUserViewModel findUser)
        {
            try
            {
                IEnumerable<UserDTO> users = await UserService
                    .GetUsers(findUser?.Name, findUser?.Surname, findUser?.Country, findUser?.City);
                if (users == null) throw new ValidationException("Users not found.", "");
                var user = GetCurrentUser();
                return Ok(users.Where(x => x.Id != user.Id));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [AllowAnonymous]
        [HttpDelete]
        [Route("{userId}")]
        public async Task<IHttpActionResult> DeleteUser([FromUri] string userId)
        {
            try
            {
                //var user = GetCurrentUser();
                //if (user == null || user.Id != userId) throw new ValidationException("Current user not found", "");
                await UserService.DeleteUser(userId);
                return Ok();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("{userId}/uploadImage")]
        public async Task<IHttpActionResult> UploadImage([FromUri] string userId)
        {
            try
            {
                var user = GetCurrentUser();
                if (user == null || user.Id != userId) throw new ValidationException("Current user not found", "");
                HttpRequest httpRequest = HttpContext.Current.Request;
                HttpPostedFile postedFile = httpRequest.Files["Image"];
                if (postedFile == null) throw new ValidationException("File has not been attached", "");
                if ((postedFile.ContentType.Contains("jpg") || postedFile.ContentType.Contains("png") || postedFile.ContentType.Contains("jpeg")) == false)
                {
                    throw new ValidationException("The file has the wrong format.", postedFile.ContentType);
                }
                var ImageUrl = "C:/Users/Dima/Documents/Programming/Angular/QwertyAngular/src/assets/ProfileImages/" + postedFile.FileName;
                postedFile.SaveAs(ImageUrl);
                OperationDetails operationDetails = await UserService.UploadImage(ImageUrl, user.UserName);
                return Ok(operationDetails);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> ChangeUser([FromBody] UserProfileViewModel userModel)
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
                    Id = GetCurrentUser().Id
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
                return InternalServerError();
            }

        }


        [HttpGet]
        [Route("{userId}")]
        public IHttpActionResult GetUser([FromUri]string userId)
        {
            try
            {
                var user = GetCurrentUser();
                if (user == null || user.Id != userId) throw new ValidationException("Current user not found", "");
                return Ok(user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}