namespace MovieCatalogAPI.Models.Core_data
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<Movie>? Movies { get; set; }
    }
}
