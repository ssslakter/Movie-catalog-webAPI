using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MovieCatalogAPI.Models
{
    public enum Gender
    {
        woman = 0,
        man = 1
    }
    public class User
    {

        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public string? AvatarLink { get; set; }
        public List<Movie>? FavoriteMovies { get; set; }
        public List<Review>? Reviews { get; set; }
        public string Role { get; set; }
    }
}
