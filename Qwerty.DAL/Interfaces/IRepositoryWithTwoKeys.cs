using System.Collections.Generic;

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
