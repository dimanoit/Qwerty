using AutoMapper;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.WEB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qwerty.WebApi.Filters;
using Serilog;
using Microsoft.AspNetCore.SignalR;
using Qwerty.WebApi.HubConfig;
using Qwerty.WebApi.InMemoryCache.Interfaces;

namespace Qwerty.WEB.Controllers
{
    [Authorize(Roles = "user", AuthenticationSchemes = "Bearer")]
    [Route("api/messages")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IUserConnectionsManager _userConnectionsManager;
        private readonly IHubContext<NotificationHub> _hub;

        public MessageController(
            IMessageService messageService,
            IUserService userService,
            IHubContext<NotificationHub> hub,
            IUserConnectionsManager userConnectionsManager)
        {
            _messageService = messageService;
            _userService = userService;
            _hub = hub;
            _userConnectionsManager = userConnectionsManager;
        }

        [HttpDelete]
        [Route("{messageId}")]
        public async Task<ActionResult> DeleteMessage(int messageId)
        {
            await _messageService.DeleteMessage(messageId);
            return Ok();
        }

        [Route("{userId}/dialogs")]
        [HttpGet]
        [CheckCurrentUserFilter]
        public async Task<ActionResult> GetAllDialogs(string userId)
        {
            var Messages = await _messageService.GetLastMessages(userId);
            List<DialogViewModel> dialogs = null;
            if (Messages == null)
            {
                Log.Warning($"User {userId} dont have dialogs");
                return BadRequest("You dont have dialogs");
            }

            dialogs = new List<DialogViewModel>();
            foreach (var el in Messages)
            {
                UserDTO MessageUser = null;
                if (el.IdSender == userId)
                {
                    MessageUser = await _userService.FindUserByIdAsync(el.IdRecipient);
                }
                else
                {
                    MessageUser = await _userService.FindUserByIdAsync(el.IdSender);
                }

                dialogs.Add(new DialogViewModel
                {
                    Message = Mapper.Map<MessageDTO, MessageViewModel>(el),
                    ImageUrl = MessageUser.ImageUrl,
                    Name = MessageUser.Name,
                    Surname = MessageUser.Surname
                });
            }

            return Ok(dialogs);
        }

        [HttpPost]
        [ModelValidationFilter]
        public async Task<ActionResult> SendMessage([FromBody] MessageViewModel message)
        {
            var messageDTO = Mapper.Map<MessageViewModel, MessageDTO>(message);
            messageDTO.DateAndTimeMessage = DateTime.Now;
            messageDTO.IdSender = message.IdSender;

            await _messageService.Send(messageDTO);


            var recipientConnectionId = _userConnectionsManager.GetUserConnectionId(message.IdRecipient);

            if (!string.IsNullOrEmpty(recipientConnectionId))
            {
                await _hub.Clients.Client(recipientConnectionId)
                    .SendAsync(NotificationHubMethods.SendNotification, messageDTO);
            }

            return Ok();
        }

        [Route("{userId}/messages/{senderId}")]
        [HttpGet]
        [CheckCurrentUserFilter]
        public async Task<ActionResult> GetAllMessageFromSender(string userId, string SenderId)
        {
            UserDTO Sender = await _userService.FindUserByIdAsync(SenderId);
            List<MessageViewModel> AllMessages = null;
            var MessagesDTO = await _messageService.GetAllMessagesFromDialog(Sender.Id, userId);
            if (MessagesDTO == null)
            {
                Log.Warning($"User {userId} have no messages from {SenderId}");
                return BadRequest("You have no messages with this user.");
            }

            AllMessages = new List<MessageViewModel>();
            foreach (var message in MessagesDTO)
            {
                AllMessages.Add(Mapper.Map<MessageDTO, MessageViewModel>(message));
            }

            return Ok(AllMessages);
        }
    }
}