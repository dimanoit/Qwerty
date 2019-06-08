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
    public class FriendshipRequestRepository : IRepositoryWithTwoKeys<FriendshipRequest>
    {
        private ApplicationContext _database;
        public FriendshipRequestRepository(ApplicationContext applicationContext)
        {
            _database = applicationContext;
        }
        public void Create(FriendshipRequest item)
        {
            _database.Requests.Add(item);
        }

        public void Delete(string IdRecipient, string IdSender)
        {
            FriendshipRequest request = _database.Requests.Find(IdSender, IdRecipient);
            if (request != null) _database.Requests.Remove(request);
        }

        public FriendshipRequest Get(string IdRecipient, string IdSender)
        {
            return _database.Requests.Find(IdSender, IdRecipient); 
        }

        public IEnumerable<FriendshipRequest> GetAll()
        {
            return _database.Requests.ToList();
        }

        public void Update(FriendshipRequest item)
        {
            _database.Entry<FriendshipRequest>(item).State = EntityState.Modified;
        }
    }
}

