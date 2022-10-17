using Microsoft.EntityFrameworkCore;

namespace MovieCatalogAPI.Models
{
    public class MovieDBContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public MovieDBContext(DbContextOptions<MovieDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
