using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Services;
using Qwerty.DAL.Interfaces;
using Xunit;

namespace BLL.Tests
{
    public class AdminServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task BlockUserAsync_BlockingUserWithNullId_ShouldBeThrownValidationException(string userId)
        {
            //Arrange
            var db = Substitute.For<IUnitOfWork>();
            var adminService = new AdminService(db);

            //Act
            Func<Task> act = async () => { await adminService.BlockUserAsync(userId); };
            
            //Assert
            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}