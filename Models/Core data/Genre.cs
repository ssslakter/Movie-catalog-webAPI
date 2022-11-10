using System.Text.Json.Serialization;

namespace MovieCatalogAPI.Models.Core_data
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public ICollection<Movie> Movies { get; set; }
    }
}
