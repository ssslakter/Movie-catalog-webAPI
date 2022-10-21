using MovieCatalogAPI.Configurations;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.Core_data;
using MovieCatalogAPI.Models.DTO;

namespace MovieCatalogAPI.Services
{
    public interface IMovieInfoService
    {
        List<MovieElementModel> GetMovieElements(int currentPage);
        void WriteToDB(MovieElementModel movie);
    }

    public class MovieInfoService : IMovieInfoService
    {
        private MovieDBContext _dbContext;

        public MovieInfoService(MovieDBContext dbContext)
        {
            _dbContext = dbContext;
            PaginationData.TotalPageCount = (_dbContext.Movies.Count() + PaginationData.MaxItemsPerPage - 1) / PaginationData.MaxItemsPerPage;
        }

        public void WriteToDB(MovieElementModel movieDTO)
        {
            var movie = new Movie
            {
                Name = movieDTO.Name,
                Poster = movieDTO.Poster,
                Year = movieDTO.Year,
                Country = movieDTO.Country,
                Reviews = new List<Review>()
            };
            if (movieDTO.Genres != null)
            {
                movie.Genres = new List<Genre>();
                foreach (var genre in movieDTO.Genres)
                {
                    var genreInDB = _dbContext.Genres.FirstOrDefault(x => x.Id == genre.Id);
                    if (genreInDB != null)
                    {
                        movie.Genres.Add(genreInDB);
                        genreInDB.Movies?.Add(movie);
                    }
                    else
                    {
                        _dbContext.Genres.Add(new Genre
                        {
                            Id = genre.Id,
                            Name = genre.Name,
                            Movies = new List<Movie> { movie }
                        });
                    }
                }
            }
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
        }

        public List<MovieElementModel> GetMovieElements(int currentPage)
        {
            return _dbContext.Movies.OrderBy(x => x.Id).Skip((currentPage - 1) * PaginationData.MaxItemsPerPage)
                .Take(PaginationData.MaxItemsPerPage)
                .Select(x => new MovieElementModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Poster = x.Poster,
                    Year = x.Year,
                    Country = x.Country,
                    Genres = x.Genres == null ? null : x.Genres.Select(g => new GenreModel { Id = g.Id, Name = g.Name }).ToList(),
                    Reviews = x.Reviews != null ? x.Reviews.Select(r => r.ToShort()).ToList() : null
                }).ToList();
        }
    }
}
