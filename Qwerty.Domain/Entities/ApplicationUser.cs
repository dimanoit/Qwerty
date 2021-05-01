using Microsoft.AspNetCore.Identity;

namespace Qwerty.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public virtual User User { get; set; }
        
    }
}
