using secure_bookstore.Dtos.Book;
using secure_bookstore.Models;

namespace secure_bookstore.Services
{
    public interface IBookService
    {
        string AddBook(AddBookDto newBook);
        List<GetBookDto> GetAllBooks();
        GetBookDto GetBookById(int id);
        string DeleteBook(int id);
        string UpdateBook(UpdateBookDto updateBook, int id);
    }
}