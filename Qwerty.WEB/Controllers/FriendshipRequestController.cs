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
    [Authorize(Roles ="user")]
    [RoutePrefix("api/friendshipRequest")]
    public class FriendshipRequestController : ApiController
    {
        public IUserService UserService => Request.GetOwinContext().GetUserManager<IUserService>();
        private IFriendService _friendService;
        private IFriendshipRequestService _friendshipRequestService;

        private async Task<UserDTO> GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            var UserName = IdentityClaims.FindFirst("sub").Value;
            return await UserService.FindUserByUsername(UserName);
        }

        public FriendshipRequestController(IFriendService friendService, IFriendshipRequestService friendshipRequestService)
        {
            _friendService = friendService;
            _friendshipRequestService = friendshipRequestService;
        }

        [HttpPost]
        public async Task<IHttpActionResult> SendFriendshipRequest([FromBody] FriendshipRequestViewModel friendshipRequestViewModel)
        {
            UserDTO Sender = await UserService.FindUserById(friendshipRequestViewModel.SenderUserId);
            if (Sender != null)
            {
                var CurrentUser = await GetCurrentUser();
                if (CurrentUser.Id == Sender.Id)
                {
                    UserDTO Recipient = await UserService.FindUserById(friendshipRequestViewModel.RecipientUserId);
                    if (Recipient != null)
                    {
                        FriendDTO friend = await _friendService.FindFriend(friendshipRequestViewModel.SenderUserId, friendshipRequestViewModel.RecipientUserId);
                        if (friend == null)
                        {
                            OperationDetails operationDetails = await _friendshipRequestService
                                .Send(Mapper.Map<FriendshipRequestViewModel, FriendshipRequestDTO>(friendshipRequestViewModel));
                            return Ok(operationDetails);
                        }
                    }
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllRequests([FromUri]string userId)
        {
            UserDTO user = await GetCurrentUser();
            if (user?.Id == userId)
            {
                var requests = await _friendshipRequestService.GetAllRequests(userId);
                if (requests != null)
                {
                    List<RequestProfile> requestProfiles = new List<RequestProfile>();
                    foreach (var el in requests)
                    {
                        UserDTO profile;
                        if (el.SenderUserId == userId) profile = await UserService.FindUserById(el.RecipientUserId);
                        else profile = await UserService.FindUserById(el.SenderUserId);
                        requestProfiles.Add(new RequestProfile()
                        {
                            Request = Mapper.Map<FriendshipRequestDTO, FriendshipRequestViewModel>(el),
                            Name = profile.Name,
                            Surname = profile.Surname,
                            ImageUrl = profile.ImageUrl
                        });
                    }
                    return Ok(requestProfiles);
                }
            }
            return BadRequest();
        }
    }
}
