using AutoMapper;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qwerty.DAL.Entities;

namespace Qwerty.WEB.Controllers
{
    [Authorize(Roles = "user", AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipRequestController : ControllerBase
    {
        public IUserService UserService;
        private IFriendService _friendService;
        private IFriendshipRequestService _friendshipRequestService;

        private async Task<UserDTO> GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            return await UserService.FindUserByIdAsync(IdentityClaims.Name);
        }

        public FriendshipRequestController(IFriendService friendService, IFriendshipRequestService friendshipRequestService, IUserService userService)
        {
            _friendService = friendService;
            _friendshipRequestService = friendshipRequestService;
            UserService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> SendFriendshipRequest([FromBody] FriendshipRequestViewModel friendshipRequestViewModel)
        {
            try
            {
                if (ModelState.IsValid == false) throw new ValidationException("Invalid request", "");
                UserDTO user = await GetCurrentUser();
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
                return null;
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllRequests(string userId)
        {
            try
            {
                if (ModelState.IsValid == false) throw new ValidationException("Invalid request", "");
                UserDTO user = await GetCurrentUser();
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
                return null;
            }
        }
    }
}
