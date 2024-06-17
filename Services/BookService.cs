using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using secure_bookstore.Data;
using secure_bookstore.Dtos.Book;
using secure_bookstore.Models;

namespace secure_bookstore.Services
{
    
    public class BookService : IBookService
    {
        private static List<Book> books = new List<Book>();
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public BookService(IMapper mapper, DataContext context, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
        }
        public string AddBook(AddBookDto newBook)
        {
            List<Book> allBooksFromDb = _context.Books.FromSqlRaw("SELECT * FROM dbo.Books").ToList();
            foreach(Book b in allBooksFromDb)
            {
                if(b.Name.Contains(newBook.Name))
                {
                    throw new Exception();
                }
                if(b.Author.Contains(newBook.Author))
                {
                    throw new Exception();
                }
            }
            Book book = new Book();
             if(ValidateString(newBook.Name) == false)
            {
                throw new Exception();
            }
            book.Name = newBook.Name;
            if(ValidateString(newBook.Author) == false)
            {
                throw new Exception();
            }
            book.Author = newBook.Author;
            Book dbBook = _mapper.Map<Book>(book);

            try
            {
                var query = "INSERT INTO dbo.Books (Name, Author) VALUES (@Name, @Author)";
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", book.Name);
                    command.Parameters.AddWithValue("@Author", book.Author);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                            // Authentication successful
                            return "Added a book.";
                        
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string DeleteBook(int id)
        {
            if(ValidateId(id) == false)
            {
                throw new Exception();
            }
            Book dbBook = _context.Books.ToList().First(b => b.Id == id);
            if(dbBook is not null)
            {
                _context.Remove(dbBook);
                _context.SaveChanges();
                return "Success!"; 
            }
            if(dbBook.Id != id)
            {
                return $"Book with Id '{id}' not found.";
            }
            throw new Exception($"Book with Id '{id}' not found.");
        }

        public List<GetBookDto> GetAllBooks()
        {
            List<Book> books = _context.Books.ToList(); //books from database
            List<GetBookDto> dtoBooks = books.Select(b => _mapper.Map<GetBookDto>(b)).ToList();
            return dtoBooks;
        }

        public GetBookDto GetBookById(int id)
        {
            if(ValidateId(id) == false)
            {
                throw new Exception("Not a valid Id.");
            }
            Book dbBook = _context.Books.ToList().FirstOrDefault(b => b.Id == id);
            
            if(dbBook is not null)
            {
                return _mapper.Map<GetBookDto>(dbBook);
            }
                    
            throw new Exception($"Book with Id '{id}' not found.");
        }

        public string UpdateBook(UpdateBookDto updateBook, int id)
        {
            if(ValidateString(updateBook.Name) == false)
            {
                throw new Exception();
            }
            if(ValidateString(updateBook.Author) == false)
            {
                throw new Exception();
            }
            if(ValidateId(id) == false)
            {
                throw new Exception();
            }
            Book dbBook = new Book();
            List<Book> allBooksFromDb = _context.Books.FromSqlRaw("SELECT * FROM dbo.Books").ToList();
            foreach(Book b in allBooksFromDb)
            {
                if(b.Id == id)
                {
                    
                    dbBook.Name = updateBook.Name;
                    dbBook.Author = updateBook.Author;

                     try
                    {
            
                        var query = "UPDATE dbo.Books SET Name = @Name, Author = @Author WHERE Id = @Id";
                        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);
                            command.Parameters.AddWithValue("@Name", dbBook.Name);
                            command.Parameters.AddWithValue("@Author", dbBook.Author);

                            connection.Open();

                            using (var reader = command.ExecuteReader())
                            {
                                return "Updated a book.";
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                        return ex.Message;
                    }
                }
                
                
                
            }
            throw new Exception($"Book with Id '{id}' not found.");
        }

        public bool ValidateString(string text)
        {
            
            bool matchText = Regex.IsMatch(text, @"^[a-zA-Z a-zA-Z]+$");
            bool lengthoftext = text.Length < 1 || text.Length > 20;
            if(matchText == false || text=="" || lengthoftext == true)
            {
                return false;
            }
               
            return true;
        }

        public bool ValidateId(int id)
        {
            string castId = id.ToString();
            bool matchId = Regex.IsMatch(castId, @"^[a-zA-Z0-9_]+$");
            bool lengthofid = castId.Length < 1 || castId.Length > 10;
            if(matchId == false || castId=="" || lengthofid == true)
            {
                return false;
            }
               
            return true;
        }
    }
}