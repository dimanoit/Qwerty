using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.DAL.Interfaces
{
    public interface IRepository<TClass,TId> where TClass : class
    {
        TClass Get(TId id);
        void Create(TClass item);
        void Update(TClass item);
        void Delete(TId id);
    }
}
