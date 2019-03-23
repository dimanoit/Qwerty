using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Entities;
namespace Qwerty.DAL.Interfaces
{
    public interface IFriendshipRequestRepository
    {
        FriendshipRequest Get(string IdRecipient,string IdSender);
        void Create(FriendshipRequest item);
        void Update(FriendshipRequest item);
        void Delete(string IdRecipient, string IdSender);
        IEnumerable<FriendshipRequest> GetAll();
    }
}
