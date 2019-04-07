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
    [Authorize]
    [RoutePrefix("api/Messages")]
    public class MessageController : ApiController
    {
       
        private async Task<UserDTO> GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            var UserName = IdentityClaims.FindFirst("sub").Value;
            return await UserService.FindUserByUsername(UserName);
        }
        public IUserService UserService => Request.GetOwinContext().GetUserManager<IUserService>();
        private IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [Route("LastMessages")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLastReceivedMessages()
        {
            UserDTO user = await GetCurrentUser();
            if (user != null)
            {
                var Messages = await _messageService.GetLastMessages(user.Id);
                List<DialogViewModel> dialogs = null;
                if (Messages != null)
                {
                    dialogs = new List<DialogViewModel>();
                    foreach (var el in Messages)
                    {
                        user = await UserService.FindUserById(el.IdSender);
                        dialogs.Add(new DialogViewModel
                        {
                            LastMessage = Mapper.Map<MessageDTO, MessageViewModel>(el),
                            ImageUrl = user.ImageUrl,
                            Name = user.Name,
                            Surname = user.Surname
                        });
                    }
                    return Ok(dialogs);
                }
            }
            return BadRequest();
        }

        [Route("SendMessage")]
        [HttpPost]
        public async Task<IHttpActionResult> SendMessage([FromBody] MessageDTO message)
        {
            UserDTO user = await GetCurrentUser();
            if (user != null)
            {
                MessageDTO messageS = message;
                messageS.DateAndTimeMessage = DateTime.Now;
                messageS.IdSender = user.Id;
                Type type = _messageService.GetType();
                OperationDetails details = await _messageService.Send(message);
                return Ok(details);
            }
            else return BadRequest();
        }
    }
}
