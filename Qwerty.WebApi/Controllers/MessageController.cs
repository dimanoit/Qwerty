using AutoMapper;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.WEB.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qwerty.DAL.Entities;
using Microsoft.AspNetCore.Http;

namespace Qwerty.WEB.Controllers
{
    [Authorize(Roles = "user", AuthenticationSchemes = "Bearer")]
    [Route("api/messages")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private async Task<UserDTO> GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            return await UserService.FindUserByIdAsync(IdentityClaims.Name);
        }

        public IUserService UserService;
        private IMessageService _messageService;

        public MessageController(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            UserService = userService;
        }

        [HttpDelete]
        [Route("{messageId}")]
        public async Task<ActionResult> DeleteMessage(int messageId)
        {
            try
            {
                OperationDetails details = await _messageService.DeleteMessage(messageId);
                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(details.Message));
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

        [Route("{userId}/dialogs")]
        [HttpGet]
        public async Task<ActionResult> GetAllDialogs(string userId)
        {
            try
            {
                UserDTO user = await GetCurrentUser();
                if (user == null || user.Id != userId) throw new ValidationException("Invalid request", "");
                var Messages = await _messageService.GetLastMessages(user.Id);
                List<DialogViewModel> dialogs = null;
                if (Messages == null) throw new ValidationException("You dont have dialogs", "");
                dialogs = new List<DialogViewModel>();
                foreach (var el in Messages)
                {
                    UserDTO MessageUser = null;
                    if (el.IdSender == user.Id) MessageUser = await UserService.FindUserByIdAsync(el.IdRecipient);
                    else MessageUser = await UserService.FindUserByIdAsync(el.IdSender);
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
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception unpredicatbleException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, unpredicatbleException.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage([FromBody] MessageViewModel message)
        {
            try
            {
                if (ModelState.IsValid == false) throw new ValidationException("Invalid request", "");
                UserDTO user = await GetCurrentUser();
                if (user == null || user.Id != message.IdSender) throw new ValidationException("Invalid request", "");
                MessageDTO messageS = Mapper.Map<MessageViewModel, MessageDTO>(message);
                messageS.DateAndTimeMessage = DateTime.Now;
                messageS.IdSender = user.Id;
                OperationDetails details = await _messageService.Send(messageS);
                return Ok(details);
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

        [Route("{userId}/messages/{senderId}")]
        [HttpGet]
        public async Task<ActionResult> GetAllMessageFromSender(string userId, string SenderId)
        {
            try
            {
                UserDTO user = await GetCurrentUser();
                if (user == null || user.Id != userId) throw new ValidationException("Invalid request", "");
                UserDTO Sender = await UserService.FindUserByIdAsync(SenderId);
                List<MessageViewModel> AllMessages = null;
                var MessagesDTO = await _messageService.GetAllMessagesFromDialog(Sender.Id, user.Id);
                if (MessagesDTO == null) throw new ValidationException("You have no messages with this user.", "");
                AllMessages = new List<MessageViewModel>();
                foreach (var message in MessagesDTO)
                {
                    AllMessages.Add(Mapper.Map<MessageDTO, MessageViewModel>(message));
                }
                return Ok(AllMessages);
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
