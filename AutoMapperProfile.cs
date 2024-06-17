using AutoMapper;
using secure_bookstore.Dtos.Book;
using secure_bookstore.Models;
namespace secure_bookstore
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Book,GetBookDto>();
            CreateMap<Book,AddBookDto>(); 
            CreateMap<GetBookDto,Book>();
            CreateMap<AddBookDto,Book>();
            CreateMap<UpdateBookDto,Book>();
            CreateMap<Book,UpdateBookDto>();
            CreateMap<RegisterUserDto, RegisterUser>();
            CreateMap<RegisterUser, RegisterUserDto>();
            CreateMap<AddBookDto,GetBookDto>();
            CreateMap<GetBookDto,AddBookDto>(); 
        }
    }
}