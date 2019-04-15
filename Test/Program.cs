using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Repositories;
using Qwerty.DAL.Entities;
using System.Configuration;
using Qwerty.BLL.Services;
using Qwerty.BLL.Infrastructure;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            OperationDetails1();
        }
        static async void OperationDetails1()
        {
            string connection = ConfigurationManager.ConnectionStrings["Model1"].ConnectionString;
            UnitOfWork unitOfWork = new UnitOfWork(connection);
            AdminService adminService = new AdminService(unitOfWork);
            await adminService.BlockUserAsync("sdfsdf");
        }
    }
}
