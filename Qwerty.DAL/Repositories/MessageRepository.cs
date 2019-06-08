using Qwerty.DAL.EF;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Qwerty.DAL.Repositories
{
    public class MessageRepository : IRepository<Message, int>
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

        public void Delete(int id)
        {
            Message boof = _database.Messages.Find(id);
            if (boof != null) _database.Messages.Remove(boof);
        }

        public Message Get(int id)
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