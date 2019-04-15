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
    [RoutePrefix("api/friendshipRequest")]
    public class FriendshipRequestController : ApiController
    {
        public IUserService UserService => Request.GetOwinContext().GetUserManager<IUserService>();
        private IFriendService _friendService;
        private IFriendshipRequestService _friendshipRequestService;

        private UserDTO GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            var UserName = IdentityClaims.FindFirst("sub").Value;
            return UserService.FindUserByUsername(UserName);
        }

        public FriendshipRequestController(IFriendService friendService, IFriendshipRequestService friendshipRequestService)
        {
            _friendService = friendService;
            _friendshipRequestService = friendshipRequestService;
        }

        [HttpPost]
        public async Task<IHttpActionResult> SendFriendshipRequest([FromBody] FriendshipRequestViewModel friendshipRequestViewModel)
        {
            try
            {
                if (ModelState.IsValid == false) throw new ValidationException("Invalid request", "");
                UserDTO user = GetCurrentUser();
                if (user == null || user.Id != friendshipRequestViewModel.SenderUserId) throw new ValidationException("Invalid request", "");
                UserDTO Recipient = await UserService.FindUserByIdAsync(friendshipRequestViewModel.RecipientUserId);
                FriendDTO friend = _friendService.FindFriend(friendshipRequestViewModel.SenderUserId, friendshipRequestViewModel.RecipientUserId);
                if (friend != null) throw new ValidationException("User is already your friend", "");
                OperationDetails operationDetails = await _friendshipRequestService
                    .Send(Mapper.Map<FriendshipRequestViewModel, FriendshipRequestDTO>(friendshipRequestViewModel));
                return Ok(operationDetails);
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

        [HttpGet]
        public async Task<IHttpActionResult> GetAllRequests([FromUri]string userId)
        {
            try
            {
                if (ModelState.IsValid == false) throw new ValidationException("Invalid request", "");
                UserDTO user = GetCurrentUser();
                if (user == null || user.Id != userId) throw new ValidationException("Invalid request", "");
                var requests = await _friendshipRequestService.GetAllRequests(userId);
                if (requests == null) throw new ValidationException("You have no friend requests.", "");
                List<RequestProfile> requestProfiles = new List<RequestProfile>();
                foreach (var el in requests)
                {
                    UserDTO profile;
                    if (el.SenderUserId == userId) profile = await UserService.FindUserByIdAsync(el.RecipientUserId);
                    else profile = await UserService.FindUserByIdAsync(el.SenderUserId);
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
