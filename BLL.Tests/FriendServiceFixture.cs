using System;
using AutoMapper;
using NSubstitute;
using Qwerty.BLL.Services;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Interfaces;

namespace BLL.Tests
{
    public class FriendServiceFixture : IDisposable
    {
        public IUnitOfWork UnitOfWork { get; }
        public FriendService FriendService { get; }

        public FriendServiceFixture()
        {
            MapperHelper.Initialize();
            UnitOfWork = Substitute.For<IUnitOfWork>();
            UnitOfWork.FriendManager.Get(Arg.Any<string>()).Returns(_ => new Friend());
            UnitOfWork.QUserManager.Get(Arg.Any<string>()).Returns(_ => new User());
            UnitOfWork.UserFriendsManager.Get(string.Empty, string.Empty).Returns(
                _ => new UserFriends
                {
                    Friend = new Friend {FriendId = "1"}
                });

            FriendService = new FriendService(UnitOfWork);
        }

        public void Dispose()
        {
            FriendService?.Dispose();
        }
    }
}