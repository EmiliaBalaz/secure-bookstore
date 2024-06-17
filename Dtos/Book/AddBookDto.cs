using System.ComponentModel.DataAnnotations;

namespace secure_bookstore.Dtos.Book
{
    public class AddBookDto
    {
         [RegularExpression("^[a-zA-Z a-zA-Z]+$", ErrorMessage="Numerics and spec characters are not allowed.")]
        public string? Name {get; set;}
         [RegularExpression("^[a-zA-Z a-zA-Z]+$", ErrorMessage="Numerics and spec characters are not allowed.")]
        public string? Author {get; set;}
    }
}

//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiQWRtaW4iLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwibmJmIjoxNzA4NzA2MDcxLCJleHAiOjE3MDg3MDcyNzEsImlhdCI6MTcwODcwNjA3MSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzI0MCIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcyNDAifQ.-2xdQ0dK--n93_coevSQyKorgcvHhXJyx4AGwKiVpGI