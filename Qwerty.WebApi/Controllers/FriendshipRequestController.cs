using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Interfaces;
using Qwerty.WEB.Models;
using Qwerty.WebApi.Filters;
using Serilog;

namespace Qwerty.WebApi.Controllers
{
    [Authorize(Roles = "user", AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipRequestController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFriendService _friendService;
        private readonly IFriendshipRequestService _friendshipRequestService;

        public FriendshipRequestController(IFriendService friendService,
            IFriendshipRequestService friendshipRequestService, IUserService userService)
        {
            _friendService = friendService;
            _friendshipRequestService = friendshipRequestService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> SendFriendshipRequest(
            [FromBody] FriendshipRequestViewModel friendshipRequestViewModel)
        {
            var friendship = await _friendService.Get(friendshipRequestViewModel.RecipientUserId,
                friendshipRequestViewModel.SenderUserId);

            if (friendship != null)
            {
                return BadRequest("User is already your friend");
            }

            var friendshipRequestDto =
                Mapper.Map<FriendshipRequestViewModel, FriendshipRequestDTO>(friendshipRequestViewModel);

            await _friendshipRequestService
                .Send(friendshipRequestDto);


            return Ok();
        }

        [HttpGet]
        [CheckCurrentUserFilter]
        public async Task<ActionResult> GetAllRequests(string userId)
        {
            var requests = await _friendshipRequestService.GetAll(userId);
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