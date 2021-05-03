namespace Qwerty.DAL.Entities
{
    public class UserFriends
    {
        public string UserId { get; set; }

        public string FriendId { get; set; }

        public User User { get; set; }

        public Friend Friend { get; set; }
    }
}