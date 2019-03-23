using Qwerty.DAL.EF;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.DAL.Repositories
{
    public class MessageRepository : IRepository<Message, string>
    {
        private ApplicationContext _database;
        public MessageRepository(ApplicationContext applicationContext)
        {
            _database = applicationContext;
        }

        public void Create(Message item)
        {
            _database.Messages.Add(item);
        }

        public void Delete(string id)
        {
            Message boof = _database.Messages.Find(id);
            if (boof != null) _database.Messages.Remove(boof);
        }

        public Message Get(string id)
        {
            return _database.Messages.Find(id);
        }

        public IEnumerable<Message> GetAll()
        {
            return _database.Messages.ToList();
        }

        public void Update(Message item)
        {
            _database.Entry(item).State = EntityState.Modified;
        }
    }
}