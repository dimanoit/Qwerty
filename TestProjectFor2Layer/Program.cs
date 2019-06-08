using System;
using  Qwerty.BLL;
using Qwerty.BLL.Services;
using Qwerty.DAL.Repositories;

namespace TestProjectFor2Layer
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
                "data source=(LocalDb)\\MSSQLLocalDB;initial catalog=TestDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            UnitOfWork uow = new UnitOfWork(connectionString);
            MessageService messageService = new MessageService(uow);
            Console.WriteLine(messageService.GetMessage(4));
            Console.Read();
        }
    }
}
