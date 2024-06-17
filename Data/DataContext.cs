using Microsoft.EntityFrameworkCore;
using secure_bookstore.Models;

namespace secure_bookstore.Data
{
    public class DataContext : DbContext
    {
      
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<RegisterUser> RegisterUsers => Set<RegisterUser>();
    }
}