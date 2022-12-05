namespace MovieCatalogAPI.Models
{
    public class ReviewShort
    {
        public Guid Id { get; set; }

        public int Rating { get; set; }

        public ReviewShort(int rating)
        {
            Rating = rating;
        }
    }
}
