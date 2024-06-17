using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using secure_bookstore.Dtos.Book;
using secure_bookstore.Models;
using secure_bookstore.Services;

[ApiController]
[Route("api/[controller]")]
[ApiKeyAuthorization]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly IConfiguration _configuration;

    public BookController(IBookService bookService, IConfiguration configuration)
    {
        _bookService = bookService;
        _configuration = configuration;
    }

    [HttpPost("add")]
    [Authorize(Roles = "Admin")]
    public ActionResult<string> AddBook(AddBookDto newBook)
    {
        if(newBook == null || !IsValidInput(newBook))
        {
            return BadRequest("Invalid filename.");
        }
            try
            {
                return Ok(_bookService.AddBook(newBook));
            }
            catch(Exception ex)
            {
            return BadRequest(ex.Message);
            }
        
        
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<string> DeleteBook(int id)
    {
        try
        {
            return Ok(_bookService.DeleteBook(id));
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("getall")]
    [Authorize(Roles ="Admin, Visitor")]
    public ActionResult<List<GetBookDto>> GetAllBooks()
    {
        try
        {
            return Ok(_bookService.GetAllBooks());
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles ="Admin, Visitor")]
    public ActionResult<GetBookDto> GetBookById(int id)
    {
        try
        {
            return Ok(_bookService.GetBookById(id));
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles ="Admin")]
    public ActionResult<string> UpdateBook(UpdateBookDto updateBook,int id)
    {
        if(updateBook == null || !IsValidInput2(updateBook))
        {
            return BadRequest("Invalid filename.");
        }
        try
        {
            if(id <= 0)
            {
                return BadRequest("Invalid id.");
            }
            return Ok(_bookService.UpdateBook(updateBook, id));
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private bool IsValidInput(AddBookDto model)
    {
         return model != null && !string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Author);
    }
    private bool IsValidInput2(UpdateBookDto model)
    {
         return model != null && !string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Author);
    }
}