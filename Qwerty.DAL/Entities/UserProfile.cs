using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.DAL.Entities
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public string AboutUrl { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public virtual Friend ProfileAsFriend { get; set; }
    }
}
