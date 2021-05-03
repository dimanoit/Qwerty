using Microsoft.AspNetCore.Identity;

namespace Qwerty.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public User User { get; set; }
    }
}