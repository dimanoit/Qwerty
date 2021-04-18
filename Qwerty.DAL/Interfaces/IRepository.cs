using System.Collections.Generic;

namespace Qwerty.DAL.Interfaces
{
    public interface IRepository<TClass,TId> where TClass : class
    {
        TClass Get(TId id);
        void Create(TClass item);
        void Update(TClass item);
        void Delete(TId id);
        IEnumerable<TClass> GetAll();
    }
}
