using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using System;
using System.Collections.Generic;
using Qwerty.WebApi.Filters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Qwerty.WEB.Controllers
{
    [Authorize(Roles = "user", AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private IUserService _userService;
        private IFriendService _friendService;
        private IMessageService _messageService;

        public FriendsController(IFriendService friendService, IMessageService messageService, IUserService userService)
        {
            _friendService = friendService;
            _messageService = messageService;
            _userService = userService;
        }

        [HttpGet]
        [CheckCurrentUserFilter]
        public async Task<ActionResult> GetAllFriendsAccounts(string userId)
        {
            IEnumerable<UserDTO> friends = await _friendService.GetFriendsProfiles(userId);
            return Ok(friends);
        }

        [HttpPost]
        [Route("{SenderId}/friend/{RecepientId}")]
        public async Task<ActionResult> AcceptFriend(string SenderId, string userId)
        {
            OperationDetails operationDetails = await _friendService.AcceptFriend(SenderId, userId);
            if (operationDetails.Succedeed == false)
            {
                Log.Warning($"Fail on accepting friend.Recipient {userId}.Sender {SenderId}");
                return BadRequest("Fail on accepting friend");
            }

            OperationDetails detailsMesage = await _messageService.Send(new MessageDTO()
            {
                DateAndTimeMessage = DateTime.Now,
                IdSender = userId,
                IdRecipient = SenderId,
                TextMessage = "Hi"
            });
            Log.Information($"User {userId} accepted friend {SenderId}");
            return Ok(detailsMesage);
        }
    }
}
