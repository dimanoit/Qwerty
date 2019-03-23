using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Interfaces;
using Qwerty.DAL.Entities;
using Qwerty.DAL.EF;

namespace Qwerty.DAL.Repositories
{
    public class UserRepository : IRepository<User,string>
    {
        private ApplicationContext _database;
        public UserRepository(ApplicationContext applicationContext)
        {
            _database = applicationContext;
        }
        public void Create(User item)
        {
            _database.Users.Add(item);
        }

        public void Delete(string id)
        {
            User boof = _database.
        }

        public User Get(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(User item)
        {
            throw new NotImplementedException();
        }
    }
}
