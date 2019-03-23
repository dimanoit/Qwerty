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
    public class UserProfileRepository : IRepository<UserProfile, string>
    {
        private ApplicationContext _database;
        public UserProfileRepository(ApplicationContext applicationContext)
        {
            _database = applicationContext;
        }
        public void Create(UserProfile item)
        {
            _database.Profiles.Add(item);
        }

        public void Delete(string id)
        {
            UserProfile boof = _database.Profiles.Find(id);
            if (boof != null)
                _database.Profiles.Remove(boof);
        }

        public UserProfile Get(string id)
        {
            return _database.Profiles.Find(id);
        }

        public IEnumerable<UserProfile> GetAll()
        {
            return _database.Profiles.ToList();
        }

        public void Update(UserProfile item)
        {
            _database.Entry(item).State = EntityState.Modified;
        }
    }
}
