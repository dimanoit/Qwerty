using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Entities;
namespace Qwerty.DAL.Interfaces
{
    public interface IRepositoryWithTwoKeys<T> where T : class
    {
        T Get(string IdRecipient, string IdSender);
        void Create(T item);
        void Update(T item);
        void Delete(string IdLeft, string IdRight);
        IEnumerable<T> GetAll();
    }
}
