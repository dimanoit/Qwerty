using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
    public class AdminServiceTest
    {
        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task BlockUserAsync_BlockingUserWithNullId_ShouldBeThrownValidationException()
        {
            Mock<IUnitOfWork> DBmock = new Mock<IUnitOfWork>();
            AdminService adminService = new AdminService(DBmock.Object);
            await adminService.BlockUserAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task BlockUserAsync_BlockingUserEmptyId_ShouldBeThrownValidationException()
        {
            Mock<IUnitOfWork> DBmock = new Mock<IUnitOfWork>();
            AdminService adminService = new AdminService(DBmock.Object);
            await adminService.BlockUserAsync("");
        }

      

    }
}
