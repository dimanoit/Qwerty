using AutoMapper;
using Microsoft.AspNet.Identity.Owin;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.WEB.Models;
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
    [Authorize(Roles = "user")]
    [RoutePrefix("api/Friends")]
    public class FriendsController : ApiController
    {
        public IUserService UserService => Request.GetOwinContext().GetUserManager<IUserService>();
        private IFriendService _friendService;
        private IMessageService _messageService;


        private UserDTO GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            var UserName = IdentityClaims.FindFirst("sub").Value;
            return UserService.FindUserByUsername(UserName);
        }

        public FriendsController(IFriendService friendService, IMessageService messageService,
            IFriendshipRequestService friendshipRequestService)
        {
            _friendService = friendService;
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllFriendsAccounts([FromUri] string userId)
        {
            try
            {
                UserDTO user = GetCurrentUser();
                if (user == null || user.Id != userId) throw new ValidationException("Invalid request", "");
                IEnumerable<UserDTO> friends = await _friendService.GetFriendsProfiles(user.Id);
                return Ok(friends);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest("Server is not responding.");
            }
        }

        [HttpPost]
        [Route("{SenderId}/friend/{RecepientId}")]
        public async Task<IHttpActionResult> AcceptFriend([FromUri] string SenderId, [FromUri] string RecepientId)
        {
            try
            {
                UserDTO user = GetCurrentUser();
                if (user == null || user.Id != RecepientId) throw new ValidationException("Invalid request", "");
                OperationDetails operationDetails = await _friendService.AcceptFriend(SenderId, RecepientId);
                if (!operationDetails.Succedeed) throw new ValidationException("Invalid request", "");
                OperationDetails detailsMesage = await _messageService.Send(new MessageDTO()
                {
                    DateAndTimeMessage = DateTime.Now,
                    IdSender = RecepientId,
                    IdRecipient = SenderId,
                    TextMessage = "Hi"
                });
                if (!detailsMesage.Succedeed) throw new ValidationException("Added in friend but" + detailsMesage.Message, "");
                return Ok(detailsMesage);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest("Server is not responding.");
            }
        }

    }
}
