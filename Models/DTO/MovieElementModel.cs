using MovieCatalogAPI.Models.DTO;

namespace MovieCatalogAPI.Models
{
    public class MovieElementModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Poster { get; set; }
        public int Year { get; set; }
        public string? Country { get; set; }
        public List<GenreModel>? Genres { get; set; }
        public List<ReviewShort>? Reviews { get; set; }
    }
}
