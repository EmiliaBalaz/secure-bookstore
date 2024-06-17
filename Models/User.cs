using System.ComponentModel.DataAnnotations;

namespace secure_bookstore.Models
{
    public class User
    {
        [Required]
        public string? UserName {get; set;}
        [Required]
        public string? Password {get; set;}
    }
}