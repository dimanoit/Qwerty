using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Services;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Interfaces;
using Xunit;

namespace BLL.Tests
{
    [Obsolete("Should update mapper DI registration")]
    public class FriendServiceTests : IClassFixture<FriendServiceFixture>
    {
        private readonly FriendServiceFixture _friendServiceFixture;

        public FriendServiceTests(FriendServiceFixture friendServiceFixture)
        {
            _friendServiceFixture = friendServiceFixture;
        }


        public void FindFriends_FindingExist_ReturnedFriendship()
        {
            //Arrange
            var expectedUserId = "1";
            var friendService =
                new FriendService(_friendServiceFixture.UnitOfWork, _friendServiceFixture.ApplicationContext);

            //Act
            var actualUserId = friendService
                .FindFriend(string.Empty, string.Empty)
                .FriendId;

            //Assert
            expectedUserId.Should().BeEquivalentTo(actualUserId);
        }

        [Fact]
        public void FindFriends_FindNonExistFriend_ReturnedNull()
        {
            // Arrange
            _friendServiceFixture.UnitOfWork.UserFriendsManager
                .Get(Arg.Any<string>(), Arg.Any<string>())
                .Returns(_ => new UserFriends());

            // Act
            var result = _friendServiceFixture.FriendService.FindFriend(string.Empty, string.Empty);

            // Assert
            result.Should().BeNull();
        }
    }
}