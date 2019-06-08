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
    [Route("api/Friends")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        public IUserService UserService;
        private IFriendService _friendService;
        private IMessageService _messageService;


        private UserDTO GetCurrentUser()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            var UserName = IdentityClaims.FindFirst("sub").Value;
            return UserService.FindUserByUsername(UserName);
        }

        public FriendsController(IFriendService friendService, IMessageService messageService, IUserService userService)
        {
            _friendService = friendService;
            _messageService = messageService;
            UserService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllFriendsAccounts( string userId)
        {
            try
            {
                UserDTO user = GetCurrentUser();
                if (user == null || user.Id != userId) throw new ValidationException("Invalid request", "");
                IEnumerable<UserDTO> friends = await _friendService.GetFriendsProfiles(user.Id);
                return Ok(friends);
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

        [HttpPost]
        [Route("{SenderId}/friend/{RecepientId}")]
        public async Task<ActionResult> AcceptFriend( string SenderId,  string RecepientId)
        {
            try
            {
                UserDTO user = GetCurrentUser();
                if (user == null || user.Id != RecepientId) throw new ValidationException("Invalid request", "");
                OperationDetails operationDetails = await _friendService.AcceptFriend(SenderId, RecepientId);
                if (!operationDetails.Succedeed) throw new ValidationException("Invalid request", "");
                OperationDetails detailsMesage = await _messageService.Send(new MessageDTO()
                {
                    DateAndTimeMessage = DateTime.Now,
                    IdSender = RecepientId,
                    IdRecipient = SenderId,
                    TextMessage = "Hi"
                });
                if (!detailsMesage.Succedeed) throw new ValidationException("Added in friend but" + detailsMesage.Message, "");
                return Ok(detailsMesage);
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
