using System.ComponentModel.DataAnnotations;

namespace Qwerty.WEB.Models
{
    public class FriendViewModel
    {
        [Required]
        public string FriendId { get; set; }
    }
}