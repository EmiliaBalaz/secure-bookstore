using System.ComponentModel.DataAnnotations;

namespace secure_bookstore.Models
{
    public class RegisterUser
{
    public int Id {get; set;}
    [Required]
    public string? FirstName {get; set;}
    [Required]
    public string? LastName {get; set;}
    [Required]
    public string? Email {get; set;}
    [Required]
    public string? UserName {get; set;}
    [Required]
    public string? Password {get; set;}
    public UserType Type {get; set;}
}
}
