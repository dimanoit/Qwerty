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
    [RoutePrefix("api/messages")]
    public class MessageController : ApiController
    {
        private UserDTO GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            var UserName = IdentityClaims.FindFirst("sub").Value;
            return UserService.FindUserByUsername(UserName);
        }
        public IUserService UserService => Request.GetOwinContext().GetUserManager<IUserService>();
        private IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpDelete]
        [Route("{messageId}")]
        public async Task<IHttpActionResult> DeleteMessage([FromUri]int messageId)
        {
            try
            {
                OperationDetails details = await _messageService.DeleteMessage(messageId);
                return Ok(details.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("{userId}/dialogs")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllDialogs([FromUri]string userId)
        {
            try
            {
                UserDTO user = GetCurrentUser();
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
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> SendMessage([FromBody] MessageViewModel message)
        {
            try
            {
                if (ModelState.IsValid == false) throw new ValidationException("Invalid request", "");
                UserDTO user = GetCurrentUser();
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
                return InternalServerError();
            }
        }

        [Route("{userId}/messages/{senderId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllMessageFromSender([FromUri]string userId, [FromUri] string SenderId)
        {
            try
            {
                UserDTO user = GetCurrentUser();
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
                return InternalServerError();
            }
        }

    }
}
