using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Services;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.BLL.Test
{
    [TestClass]
    public class MessageServiceTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Message, MessageDTO>().ReverseMap());
        }

        [TestMethod]
        public async Task SendMessage_SendNewMessage_ShouldBeSentMessage()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.MessageManager.Get(It.IsAny<int>())).Returns<Message>(null);
            MessageService service = new MessageService(uow.Object);

            await service.Send(new MessageDTO()
            {
                IdMessage = It.IsAny<int>(),
                IdRecipient = It.IsAny<string>(),
                IdSender = It.IsAny<string>(),
                TextMessage = "123"
            });

            uow.Verify(x => x.SaveAsync());
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task SendMessage_SendExistMessage_ShouldBeThrownValidationException()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.MessageManager.Get(It.IsAny<int>())).Returns(new Message());
            MessageService service = new MessageService(uow.Object);

            await service.Send(It.IsAny<MessageDTO>());

        }
    }
}
