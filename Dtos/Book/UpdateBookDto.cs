using System.ComponentModel.DataAnnotations;

namespace secure_bookstore.Dtos.Book
{
    public class UpdateBookDto
    {
        [RegularExpression("^[a-zA-Z a-zA-Z]+$", ErrorMessage="Numerics and spec characters are not allowed.")]
        public string? Name {get; set;}
        [RegularExpression("^[a-zA-Z a-zA-Z]+$*", ErrorMessage="Numerics and spec characters are not allowed.")]
        public string? Author {get; set;}
    }
}