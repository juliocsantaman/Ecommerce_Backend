using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_Backend.Models
{
    [Table("User")]
    public class User
    {
        //[Key]
        //public Guid UserId { get; set; }
        [Key]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public DateTime Created {  get; set; }
    }
}

