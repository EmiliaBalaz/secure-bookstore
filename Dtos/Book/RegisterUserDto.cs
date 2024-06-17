using System.ComponentModel.DataAnnotations;

namespace secure_bookstore.Dtos.Book
{
    public class RegisterUserDto
    {
    [Required]
     [RegularExpression("[a-zA-Z]+[a-zA-Z]*", ErrorMessage="Numerics and spec characters are not allowed.")]
    public string? FirstName {get; set;}
    [Required]
     [RegularExpression("[a-zA-Z]+[a-zA-Z]*", ErrorMessage="Numerics and spec characters are not allowed.")]
    public string? LastName {get; set;}
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage ="Email is not in a valid form.")]
    public string? Email {get; set;}
    [Required]
     [RegularExpression("[a-zA-Z]+[a-zA-Z]*", ErrorMessage="Numerics and spec characters are not allowed.")]
    public string? UserName {get; set;}
    [Required]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage ="Incorrect password.")]
    public string? Password {get; set;}
    [Required]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage ="Incorrect password.")]
    public string? PasswordVerify {get; set;} 
    }
}