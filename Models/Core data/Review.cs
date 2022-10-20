using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieCatalogAPI.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        [Required]
        public Movie Movie { get; set; }
        public string? ReviewText { get; set; }
        public int Rating { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime CreateDateTime { get; set; }
        [Required]
        public User Author { get; set; }

        public ReviewShort ToShort()
        {
            return new ReviewShort(Rating)
            {
                Id = Id
            };
        }
    }
}
