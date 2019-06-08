using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Interfaces;
using Qwerty.DAL.Entities;
using Qwerty.DAL.EF;
using Microsoft.EntityFrameworkCore;

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
            _database.QUsers.Add(item);
        }

        public void Delete(string id)
        {
            User boof = _database.QUsers.Find(id);
            if (boof != null)
                _database.QUsers.Remove(boof);
        }

        public User Get(string id)
        {
            return _database.QUsers.Find(id);
        }

        public IEnumerable<User> GetAll()
        {
            return _database.QUsers.ToList();
        }

        public void Update(User item)
        {
            _database.Entry(item).State = EntityState.Modified;
        }
    }
}
