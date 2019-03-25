using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Repositories;
using System.Configuration;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string connection = ConfigurationManager.ConnectionStrings["Model1"].ConnectionString;
            UnitOfWork unitOfWork = new UnitOfWork(connection);
            unitOfWork.QUserManager.GetAll();
            Console.ReadLine();
        }
    }
}
