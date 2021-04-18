using System.ComponentModel.DataAnnotations;

namespace Qwerty.WEB.Models
{
    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(3)]
        public string Password { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public string SurName { get; set; }
    }
}