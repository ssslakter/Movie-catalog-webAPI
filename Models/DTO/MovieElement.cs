namespace MovieCatalogAPI.Models
{
    public class MovieElement
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Poster { get; set; }
        public int Year { get; set; }
        public string? Country { get; set; }
        public List<Genre>? Genres { get; set; }
        public List<ReviewShort>? Reviews { get; set; }
    }
}
