namespace MovieCatalogAPI.Models
{
    public class ReviewShort
    {
        public string Id { get; set; }

        public int Rating { get; set; }

        public ReviewShort(int rating)
        {
            Rating = rating;
        }
    }
}
