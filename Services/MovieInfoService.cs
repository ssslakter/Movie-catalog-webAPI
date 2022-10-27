using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Configurations;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.Core_data;
using MovieCatalogAPI.Models.DTO;

namespace MovieCatalogAPI.Services
{
    public interface IMovieInfoService
    {
        List<MovieElementModel> GetMovieElements(int currentPage);
        Task<MovieDetailsModel> GetMovieDetails(Guid movieId);
    }

    public class MovieInfoService : IMovieInfoService
    {
        private MovieDBContext _dbContext;
        private ILogger _logger;

        public MovieInfoService(MovieDBContext dbContext, ILogger logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            PaginationData.TotalPageCount = (_dbContext.Movies.Count() + PaginationData.MaxItemsPerPage - 1) / PaginationData.MaxItemsPerPage;
        }

        public async Task<MovieDetailsModel> GetMovieDetails(Guid movieId)
        {
            var movie = await _dbContext.Movies.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == movieId);
            if (movie == null)
            {
                _logger.Log(LogLevel.Information, $"Not found movie with id {movieId}");
                throw new KeyNotFoundException($"Not found movie with id {movieId}");
            }
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

        public List<MovieElementModel> GetMovieElements(int currentPage)
        {
            return _dbContext.Movies.OrderBy(x => x.Year).Skip((currentPage - 1) * PaginationData.MaxItemsPerPage)
                .Take(PaginationData.MaxItemsPerPage).Include(x => x.Genres).ToList()
                .Select(x => new MovieElementModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Poster = x.Poster,
                    Year = x.Year,
                    Country = x.Country,
                    Genres = x.Genres?.Select(g => new GenreModel { Id = g.Id, Name = g.Name }).ToList(),
                    Reviews = x.Reviews != null ? x.Reviews.Select(r => r.ToShort()).ToList() : null
                }).ToList();
        }
    }
}
