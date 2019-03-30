using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Qwerty.WEB.Models
{
    public class UserProfileViewModel
    {
        [Required]
        public string UserId { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string ImageUrl { get; set; }
        public string AboutUrl { get; set; }

        [RegularExpression(@"[a-zA-Z]")]
        public string Country { get; set; }

        [RegularExpression(@"[a-zA-Z]")]
        public string City { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}