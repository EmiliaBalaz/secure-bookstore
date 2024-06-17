using System.ComponentModel.DataAnnotations;

namespace secure_bookstore.Models
{
    public class LoginUserDto
    {
        [Required]
         [RegularExpression("[a-zA-Z]+[a-zA-Z]*", ErrorMessage="Numerics and spec characters are not allowed.")]
        public string? UserName {get; set;}
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage ="Incorrect password.")]
        public string? Password {get; set;}
    }
}