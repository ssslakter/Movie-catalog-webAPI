using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MovieCatalogAPI.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        [Required, JsonIgnore]
        public Movie Movie { get; set; }
        [Required]
        public string ReviewText { get; set; }
        [Range(0, 10)]
        public int Rating { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime CreateDateTime { get; set; }
        [Required, JsonIgnore]
        public User AuthorData { get; set; }
        [NotMapped]
        public UserShort Author { get; set; }
        public Review()
        {
            Author = new UserShort
            {
                UserID = AuthorData.Id,
                Avatar = AuthorData.AvatarLink,
                UserName = AuthorData.UserName
            };
        }

        public ReviewShort ToShort()
        {
            return new ReviewShort(Rating)
            {
                Id = Id
            };
        }
    }
}
