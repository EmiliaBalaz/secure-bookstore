using secure_bookstore.Models;
using AutoMapper;
using secure_bookstore.Dtos.Book;
using secure_bookstore.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace secure_bookstore.Services
{

    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IEncryptService _encrypteService;
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;

        private static string encryptedPassword;

        public UserService(IMapper mapper, DataContext context, IEncryptService encryptService, IOptions<JwtSettings> jwtSettings, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _encrypteService = encryptService;
            _jwtSettings = jwtSettings.Value;
            _configuration = configuration;
        }
        public string Login(LoginUserDto user)
        {
            if(ValidateUserName(user.UserName) == false)
            {
                throw new Exception("Numerics and spec characters are not allowed.");
            }
            RegisterUser ru = _context.RegisterUsers.FromSqlInterpolated($"SELECT * FROM dbo.RegisterUsers WHERE UserName = {user.UserName}").FirstOrDefault();
            
                    encryptedPassword = _encrypteService.EncodePassword(user.Password);
                    if(ru.Password == encryptedPassword)
                    {
                        List<Claim> claims = new List<Claim>();            
                        if(ru.Type == UserType.StaffMember)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                        }
                        else if(ru.Type == UserType.Visitor)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "Visitor"));
                        }
                        claims.Add(new Claim(ClaimTypes.Name, ru.UserName));

                        //Generate Token
                        var tokenhandler = new JwtSecurityTokenHandler();
                        var tokenkey = Encoding.UTF8.GetBytes(_jwtSettings.securitykey);
                        var tokendescriptor = new SecurityTokenDescriptor 
                        {
                            Subject = new ClaimsIdentity(claims),
                            Expires = DateTime.Now.AddMinutes(20),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256),
                            Issuer = _jwtSettings.Issuer,
                           Audience = _jwtSettings.Audience
                        };
                        var token = tokenhandler.CreateToken(tokendescriptor);
                        string finalToken = tokenhandler.WriteToken(token);

                        return finalToken;  
                    }
                    else
                    {
                        return "Incorrect password. Please try again.";
                    }
        }

        public  string Register(RegisterUserDto user)
        {
            RegisterUser registerUser = new RegisterUser();
            if(ValidateUserName(user.UserName) == false)
            {
                throw new Exception("Numerics and spec characters are not allowed.");
            }
            registerUser.UserName = user.UserName;
            if(ValidateFirstAndLastName(user.FirstName) == false)
            {
                throw new Exception("Incorrect FirstName.");
            }
            registerUser.FirstName = user.FirstName;
            if(ValidateFirstAndLastName(user.LastName) == false)
            {
                throw new Exception("Incorrect FirstName.");
            }
            registerUser.LastName = user.LastName;

            if(ValidateEmailAddress(user.Email) == false)
            {
                throw new Exception("Please enter your email address in format: yourname@example.com!");
            }
            registerUser.Email = user.Email;
            if(user.Password != user.PasswordVerify)
            {
                return "Incorrect password. Try again.";
            }
            registerUser.Password = _encrypteService.EncodePassword(user.Password);
            registerUser.Type = UserType.Visitor;

            RegisterUser regUser = _mapper.Map<RegisterUser>(registerUser);
            //List<RegisterUser> registerUsers = _context.RegisterUsers.ToList(); //users from database
            List<RegisterUser> registerUsers = _context.RegisterUsers.FromSqlRaw("EXEC GetUsers").ToList();
            foreach(RegisterUser ru in registerUsers)
            {
                if(ru.UserName == registerUser.UserName)
                {
                    throw new Exception("User with this username already exist. Please try again.");
                }
            }
            try
            {
                var query = "INSERT INTO dbo.RegisterUsers (FirstName, LastName, Email, UserName, Password, Type) VALUES (@FirstName, @LastName, @Email, @UserName, @Password, @Type)";
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", registerUser.FirstName);
                    command.Parameters.AddWithValue("@LastName", registerUser.LastName);
                    command.Parameters.AddWithValue("@Email", registerUser.Email);
                    command.Parameters.AddWithValue("@UserName", registerUser.UserName);
                    command.Parameters.AddWithValue("@Password", registerUser.Password);
                    command.Parameters.AddWithValue("@Type", registerUser.Type);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        return $"Successfully added a new user! Hi {registerUser.UserName}!"; 
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }

        public bool ValidateUserName(string userName)
        {
            bool matchUserName = Regex.IsMatch(userName, @"^[a-zA-Z0-9_]+$");
            bool lengthofuserName = userName.Length < 1 || userName.Length > 20;
            if(matchUserName == false || userName=="" || lengthofuserName == true)
            {
                return false;
            }
               
            return true;
        }

         public bool ValidateFirstAndLastName(string name)
        {
            bool matchName = Regex.IsMatch(name, @"^[a-zA-Z0-9_]+$");
            bool lengthofname = name.Length < 1 || name.Length > 20;
            if(matchName == false || name=="" || lengthofname == true)
            {
                return false;
            }
            return true;
        }

        public bool ValidateEmailAddress(string emailAddress)
        {
            //try
            //{
                //var email = new MailAddress(emailAddress);
                //return email.Address == emailAddress.Trim();
            //}
            //catch
            //{
                //return false;
            //}
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(emailPattern);
            return regex.IsMatch(emailAddress);
        }

         public bool ValidatePassword(string password)
        {
            bool passLength = password.Length < 8;
            bool oneUpperCase = !password.Any(char.IsUpper);
            bool oneLowerCase = !password.Any(char.IsLower);
            bool whiteSpace = password.Contains(" ");
            if(passLength == true ||  oneUpperCase == true || oneLowerCase == true || whiteSpace == true)
            {
                return false;
            }
            return true;
        }

        public bool ValidatePasswordLogin(string password)
        {
            bool matchPassword = Regex.IsMatch(password, @"^[a-zA-Z0-9_]+$");
            bool lengthOfPassword = password.Length < 1 || password.Length > 20;
            if(matchPassword == false || password=="" || lengthOfPassword == true)
            {
                return false;
            }
               
            return true;
        }

    }
}