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
using Qwerty.WebApi.Filters;
using Serilog;

namespace Qwerty.WEB.Controllers
{
    [Authorize(Roles = "user", AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipRequestController : ControllerBase
    {
        private IUserService _userService;
        private IFriendService _friendService;
        private IFriendshipRequestService _friendshipRequestService;

        public FriendshipRequestController(IFriendService friendService, IFriendshipRequestService friendshipRequestService, IUserService userService)
        {
            _friendService = friendService;
            _friendshipRequestService = friendshipRequestService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> SendFriendshipRequest([FromBody] FriendshipRequestViewModel friendshipRequestViewModel)
        {
            UserDTO recipient = await _userService.FindUserByIdAsync(friendshipRequestViewModel.RecipientUserId);
            FriendDTO friend = _friendService.FindFriend(friendshipRequestViewModel.SenderUserId, friendshipRequestViewModel.RecipientUserId);
            if (friend != null)
            {
                Log.Warning($"Try send friendship request, but recipient is already was friend." +
                    $"SenderId {friendshipRequestViewModel.SenderUserId}. RecipientId {friendshipRequestViewModel.RecipientUserId}");
                return BadRequest("User is already your friend");
            }
            OperationDetails operationDetails = await _friendshipRequestService
                .Send(Mapper.Map<FriendshipRequestViewModel, FriendshipRequestDTO>(friendshipRequestViewModel));
            Log.Information($"Friendship request was send from {friendshipRequestViewModel.SenderUserId} to {friendshipRequestViewModel.RecipientUserId}");
            return Ok(operationDetails);
        }

        [HttpGet]
        [CheckCurrentUserFilter]
        public async Task<ActionResult> GetAllRequests(string userId)
        {
            var requests = await _friendshipRequestService.GetAllRequests(userId);
            if (requests == null)
            {
                Log.Warning($"User {userId} dont have friend requests");
                return BadRequest("You have no friend requests.");
            }
            List<RequestProfile> requestProfiles = new List<RequestProfile>();
            foreach (var el in requests)
            {
                UserDTO profile;
                if (el.SenderUserId == userId)
                {
                    profile = await _userService.FindUserByIdAsync(el.RecipientUserId);
                }
                else
                {
                    profile = await _userService.FindUserByIdAsync(el.SenderUserId);
                }

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
}
