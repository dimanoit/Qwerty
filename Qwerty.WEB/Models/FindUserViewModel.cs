using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Qwerty.WEB.Models
{
    public class FindUserViewModel
    {
        [RegularExpression(@"[a-zA-Z]")]
        public string City { get; set; }

        [RegularExpression(@"[a-zA-Z]")]
        public string Country { get; set; }

        public string Surname { get; set; }

        public string Name { get; set; }


    }
}