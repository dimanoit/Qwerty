using Qwerty.DAL.EF;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Qwerty.DAL.Repositories
{
    public class FriendRepository : IRepository<Friend, string>
    {
        private  ApplicationContext _database;
        public FriendRepository(ApplicationContext applicationContext)
        {
            _database = applicationContext;
        }
        public void Create(Friend item)
        {
            _database.Friends.Add(item);
        }

        public void Delete(string id)
        {
            Friend boof = _database.Friends.Find(id);
            if (boof != null)
                _database.Friends.Remove(boof);
        }

        public Friend Get(string id)
        {
            return _database.Friends.Find(id);
        }

        public IEnumerable<Friend> GetAll()
        {
            return _database.Friends.ToList();
        }

        public void Update(Friend item)
        {
            _database.Entry(item).State = EntityState.Modified;
        }
    }
}
