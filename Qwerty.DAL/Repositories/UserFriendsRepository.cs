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
    public class UserFriendsRepository : IRepositoryWithTwoKeys<UserFriends>
    {
        private ApplicationContext _database;
        public UserFriendsRepository(ApplicationContext applicationContext)
        {
            _database = applicationContext;
        }

        public void Create(UserFriends item)
        {
            _database.UserFriends.Add(item);

        }
        public void Delete(string UserId, string FriendId)
        {
            UserFriends userFriends = _database.UserFriends.Find(UserId, FriendId);
            if (userFriends != null) _database.UserFriends.Remove(userFriends);
        }

        public UserFriends Get(string UserId, string FriendId)
        {
            return _database.UserFriends.Find(UserId, FriendId);
        }

        public IEnumerable<UserFriends> GetAll()
        {
            return _database.UserFriends.ToList();
        }

        public void Update(UserFriends item)
        {
            _database.Entry<UserFriends>(item).State = EntityState.Modified;
        }
    }
}
