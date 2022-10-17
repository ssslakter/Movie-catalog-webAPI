using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Services
{
    public interface IMovieInfoService
    {
        List<MovieElement> GetMovieElements();
        void WriteToDB(MovieElement movie);
    }

    public class MovieInfoService : IMovieInfoService
    {
        private MovieDBContext _dbContext;

        public MovieInfoService(MovieDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void WriteToDB(MovieElement movie)
        {
            _dbContext.Movies.Add(new Movie
            {
                Id = movie.Id,
                Name = movie.Name,
                Poster = movie.Poster,
                Year = movie.Year,
                Country = movie.Country,
                Genres = movie.Genres,
                Reviews = new List<Review>()
            });
            _dbContext.SaveChanges();
        }

        List<MovieElement> IMovieInfoService.GetMovieElements()
        {
            return _dbContext.Movies.Select(x => new MovieElement
            {
                Id = x.Id,
                Name = x.Name,
                Poster = x.Poster,
                Year = x.Year,
                Country = x.Country,
                Genres = x.Genres,
                Reviews = x.Reviews != null ? x.Reviews.Select(r => r.ToShort()).ToList() : null
            }).ToList();
        }
    }
}
