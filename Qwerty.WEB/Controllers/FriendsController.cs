using AutoMapper;
using Microsoft.AspNet.Identity.Owin;
using Qwerty.BLL.Comparators;
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
    [Authorize]
    [RoutePrefix("api/Friends")]
    public class FriendsController : ApiController
    {
        public IUserService UserService => Request.GetOwinContext().GetUserManager<IUserService>();
        private IFriendService _friendService;
        private IFriendshipRequestService _friendshipRequestService;
        private IMessageService _messageService;


        private async Task<UserDTO> GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            var UserName = IdentityClaims.FindFirst("sub").Value;
            return await UserService.FindUserByUsername(UserName);
        }

        public FriendsController(IFriendService friendService,IMessageService messageService,
            IFriendshipRequestService friendshipRequestService)
        {
            _friendService = friendService;
            _friendshipRequestService = friendshipRequestService;
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllFriendsAccounts([FromUri] string userId)
        {
            UserDTO user = await GetCurrentUser();
            if(user != null && user.Id == userId)
            {
                IEnumerable<UserDTO> friends = await _friendService.GetFriendsProfiles(user.Id);
                return Ok(friends);
            }return BadRequest();
        }

        [HttpPost]
        [Route("{SenderId}/friends/{RecepientId}")]
        public async Task<IHttpActionResult> AcceptFriend([FromUri] string SenderId, [FromUri] string RecepientId)
        {
            OperationDetails operationDetails = await _friendService.AcceptFriend(SenderId, RecepientId);
            if (operationDetails.Succedeed)
            {
                OperationDetails detailsMesage = await _messageService.Send(new MessageDTO()
                {
                    DateAndTimeMessage = DateTime.Now,
                    IdSender = RecepientId,
                    IdRecipient = SenderId,
                    TextMessage = "Hi"
                });

                return Ok(detailsMesage);
            }
            else return BadRequest();
        }
      
    }
}
