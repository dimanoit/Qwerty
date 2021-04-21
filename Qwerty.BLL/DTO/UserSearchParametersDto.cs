namespace Qwerty.BLL.DTO
{
    public class UserSearchParametersDto
    {
        public string CurrentUserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool? ExceptFriends { get; set; }
    }
}