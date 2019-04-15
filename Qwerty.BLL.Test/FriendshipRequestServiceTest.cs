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
    public class FriendshipRequestServiceTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FriendshipRequest, FriendshipRequestDTO>().ReverseMap());
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task Send_SendAlreadySendedRequest_ShouldBeThrownValidationException()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.RequestManager.Get(It.IsAny<string>(), It.IsAny<string>())).Returns(new FriendshipRequest());
            FriendshipRequestService service = new FriendshipRequestService(uow.Object);

            await service.Send(new FriendshipRequestDTO() { RecipientUserId = It.IsAny<string>(), SenderUserId = It.IsAny<string>() });
        }


        [TestMethod]
        public async Task Send_SendNewRequest_ShouldBeSendedRequest()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.RequestManager.Get(It.IsAny<string>(), It.IsAny<string>())).Returns<FriendshipRequest>(null);
            FriendshipRequestService service = new FriendshipRequestService(uow.Object);
            bool expected = true; 

            OperationDetails actual = await service.Send(new FriendshipRequestDTO() { RecipientUserId = It.IsAny<string>(), SenderUserId = It.IsAny<string>() });

            uow.Verify(x => x.SaveAsync());
            Assert.AreEqual(expected, actual.Succedeed);
        }


        [TestMethod]
        public async Task GetAllRequests_GetRequestUsersWithCorrectId_ShouldBeRecived()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            List<FriendshipRequest> Init = new List<FriendshipRequest>()
            {
                new FriendshipRequest {RecipientUserId = "2",SenderUserId = "1"},
                new FriendshipRequest {RecipientUserId = "3",SenderUserId = "1"},
                new FriendshipRequest {RecipientUserId = "4",SenderUserId = "1"},
            };
            List<FriendshipRequestDTO> expected = new List<FriendshipRequestDTO>()
            {
                new FriendshipRequestDTO {RecipientUserId = "2",SenderUserId = "1"},
                new FriendshipRequestDTO {RecipientUserId = "3",SenderUserId = "1"},
                new FriendshipRequestDTO {RecipientUserId = "4",SenderUserId = "1"},
            };
            uow.Setup(a => a.QUserManager.Get(It.IsAny<string>())).Returns(new User() { SendFriendshipRequests = Init });
            FriendshipRequestService service = new FriendshipRequestService(uow.Object);

            var actual = await service.GetAllRequests("1");

            CollectionAssert.AreEqual(expected.Select(x => x.SenderUserId).ToList(), actual.Select(x => x.SenderUserId).ToList());
        }
    }
}
