using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Configurations;
using MovieCatalogAPI.Models.DTO;
using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Services
{
    public interface IMovieConverterService
    {
        IEnumerable<MovieElementModel> MoviesToMovieElements(IEnumerable<Movie> movies);
        MovieDetailsModel MoviesToMovieDetails(Movie movie);
    }
    public class MovieConverterService: IMovieConverterService
    {
        public IEnumerable<MovieElementModel> MoviesToMovieElements(IEnumerable<Movie> movies)
        {
            return movies.Select(x => new MovieElementModel
            {
                Id = x.Id,
                Name = x.Name,
                Poster = x.Poster,
                Year = x.Year,
                Country = x.Country,
                Genres = x.Genres?.Select(g => new GenreModel { Id = g.Id, Name = g.Name }).ToList(),
                Reviews = x.Reviews?.Select(r => r.ToShort()).ToList()
            }).ToList();
        }
        public MovieDetailsModel MoviesToMovieDetails(Movie movie)
        {
            return new MovieDetailsModel
            {
                Id = movie.Id,
                Name = movie.Name,
                Poster = movie.Poster,
                Year = movie.Year,
                Country = movie.Country,
                Genres = movie.Genres?.Select(g => new GenreModel { Id = g.Id, Name = g.Name }).ToList(),
                Reviews = movie.Reviews,
                Time = movie.Time,
                Tagline = movie.Tagline,
                Description = movie.Description,
                Director = movie.Director,
                Budget = movie.Budget,
                Fees = movie.Fees,
                AgeLimit = movie.AgeLimit
            };
        }
    }
}
