
namespace Qwerty.DAL.Entities
{
    public class UserFriends
    {
        public string UserId { get; set; }

        public string FriendId { get; set; }

        public virtual User User { get; set; }

        public virtual Friend Friend { get; set; }
    }
}
