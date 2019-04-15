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
    public class FriendServiceTest 
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Friend,FriendDTO>().ReverseMap());
        }

        #region TestCreateMethod

        [TestMethod]
        public async Task Create_AddNewFriend_ShouldBeAddedFriend()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.FriendManager.Get(It.IsAny<string>())).Returns<Friend>(null);
            uow.Setup(a => a.QUserManager.Get(It.IsAny<string>())).Returns(new User());
            FriendService service = new FriendService(uow.Object);
            OperationDetails expected = new OperationDetails(true, "", "");

            OperationDetails actual = await service.Create(new FriendDTO() { FriendId = It.IsAny<string>()});

            uow.Verify(x => x.SaveAsync());
            Assert.AreEqual(expected.Succedeed, actual.Succedeed);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task Create_AddExistingFriend_ShouldBeThrownValidationException()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.FriendManager.Get(It.IsAny<string>())).Returns(new Friend());
            uow.Setup(a => a.QUserManager.Get(It.IsAny<string>())).Returns(new User());
            FriendService service = new FriendService(uow.Object);

            await service.Create(It.IsAny<FriendDTO>());
        }


        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task Create_AddFriendWithNonExistUserAccount_ShouldBeThrownValidationException()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.FriendManager.Get(It.IsAny<string>())).Returns(new Friend());
            uow.Setup(a => a.QUserManager.Get(It.IsAny<string>())).Returns<User>(null);
            FriendService service = new FriendService(uow.Object);

            await service.Create(It.IsAny<FriendDTO>());
        }


        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task Create_AddFriendWithNullFriendDTO_ShouldBeThrownValidationException()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            FriendService service = new FriendService(uow.Object);

            await service.Create(null);
        }

        #endregion

        #region TestDeleteMethod

        [TestMethod]
        public async Task Delete_ExistingFriend_ShouldBeDeletedFriend()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.FriendManager.Get(It.IsAny<string>())).Returns(new Friend());
            uow.Setup(a => a.UserFriendsManager.Get(It.IsAny<string>(), It.IsAny<string>())).Returns(new UserFriends());
            FriendService service = new FriendService(uow.Object);
            OperationDetails expected = new OperationDetails(true, "", "");

            OperationDetails actual = await service.DeleteFriend(It.IsAny<string>(), It.IsAny<string>());

            uow.Verify(x => x.SaveAsync());
            Assert.AreEqual(expected.Succedeed, actual.Succedeed);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task Delete_NonExistingFriend_ShouldBeThrownValidationException()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.FriendManager.Get(It.IsAny<string>())).Returns<Friend>(null);
            FriendService service = new FriendService(uow.Object);

            await service.DeleteFriend(It.IsAny<string>(), It.IsAny<string>());
        }


        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task Delete_FriendInNonExistFriendship_ShouldBeThrownValidationException()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.FriendManager.Get(It.IsAny<string>())).Returns(new Friend());
            uow.Setup(a => a.UserFriendsManager.Get(It.IsAny<string>(), It.IsAny<string>())).Returns<UserFriends>(null);
            FriendService service = new FriendService(uow.Object);

            await service.DeleteFriend(It.IsAny<string>(), It.IsAny<string>());
        }
        #endregion

        #region FindFriends
        [TestMethod]
        public void FindFriends_FindingExist_ReturnedFriendship()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.UserFriendsManager.Get(It.IsAny<string>(), It.IsAny<string>()).Friend)
                .Returns(new Friend() { FriendId = "1"});
            FriendService service = new FriendService(uow.Object);
            string ExpectedUserId = "1";

            string ActualUserId = service.FindFriend(It.IsAny<string>(), It.IsAny<string>()).FriendId;

            Assert.AreEqual(ExpectedUserId, ActualUserId);
        }

        [TestMethod]
        public void FindFriends_FindNonExistFriend_ReturnedNull()
        {
            Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();
            uow.Setup(a => a.UserFriendsManager.Get(It.IsAny<string>(), It.IsAny<string>()).Friend)
                .Returns<Friend>(null);
            FriendService service = new FriendService(uow.Object);

            FriendDTO friend = service.FindFriend(It.IsAny<string>(), It.IsAny<string>());

            Assert.IsNull(friend);
        }
        #endregion


    }
}
