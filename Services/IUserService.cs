using secure_bookstore.Dtos.Book;
using secure_bookstore.Models;

namespace secure_bookstore.Services
{
    public interface IUserService
    {
       string Login(LoginUserDto user);
       string Register(RegisterUserDto registerUser);

    }
}