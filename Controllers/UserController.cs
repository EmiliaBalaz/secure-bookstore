using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using secure_bookstore.Dtos.Book;
using secure_bookstore.Models;
using secure_bookstore.Services;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    public readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IEncryptService _encrypteService;


    public UserController(IUserService userService, IConfiguration configuration, IEncryptService encryptService)
    {
        _userService = userService;
        _configuration = configuration;
        _encrypteService = encryptService;
    }

    [HttpPost("login")]

    public ActionResult<string> Login(LoginUserDto user)
    {
        if (IsInvalidInput(user))
        {
            return BadRequest("Invalid input");
        }
        var query = "SELECT * FROM dbo.RegisterUsers WHERE UserName = @UserName AND Password = @Password";

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserName", user.UserName);
            command.Parameters.AddWithValue("@Password", _encrypteService.EncodePassword(user.Password));

            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    return Ok(_userService.Login(user));
                }
                else
                {
                    return Unauthorized("Invalid credentials");
                }
            }
        }
    } 

    [HttpPost("register")]
    public ActionResult<string> Register(RegisterUserDto registerUser)
    {
        try
        {    
            return Ok(_userService.Register(registerUser));
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    private bool IsInvalidInput(LoginUserDto model)
    {
        return model == null || string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password);
    }
}