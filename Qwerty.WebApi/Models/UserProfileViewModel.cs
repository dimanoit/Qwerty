using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Qwerty.WEB.Models
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string ImageUrl { get; set; }

        public string AboutUrl { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}