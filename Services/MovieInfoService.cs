using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Configurations;
using MovieCatalogAPI.Exceptions;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.Core_data;

namespace MovieCatalogAPI.Services
{
    public interface IMovieInfoService
    {
        IEnumerable<MovieElementModel> GetMovieElements(int currentPage);
        Task<MovieDetailsModel> GetMovieDetails(Guid movieId);
    }

    public class MovieInfoService : IMovieInfoService
    {
        private readonly IMovieConverterService _movieConverterService;
        private MovieDBContext _dbContext;
        private ILogger _logger;

        public MovieInfoService(MovieDBContext dbContext, ILogger<MovieInfoService> logger, IMovieConverterService movieConverterService)
        {
            _logger = logger;
            _dbContext = dbContext;
            PaginationData.TotalPageCount = (_dbContext.Movies.Count() + PaginationData.MaxItemsPerPage - 1) / PaginationData.MaxItemsPerPage;
            _movieConverterService = movieConverterService;
        }

        public async Task<MovieDetailsModel> GetMovieDetails(Guid movieId)
        {           
            var movie = await _dbContext.Movies.Include(x => x.Genres).Include(x => x.Reviews).ThenInclude(x => x.AuthorData).FirstOrDefaultAsync(x => x.Id == movieId);
            if (movie == null)
            {
                _logger.Log(LogLevel.Information, $"Not found movie with id {movieId}");
                throw new NotFoundException($"Not found movie with id {movieId}");
            }
            
            foreach (var review in movie.Reviews)
            {               
                review.AddAuthorShort();                
            }            
            return _movieConverterService.MoviesToMovieDetails(movie);
        }


        public IEnumerable<MovieElementModel> GetMovieElements(int currentPage)
        {
            if (currentPage > PaginationData.TotalPageCount)
            {
                return new List<MovieElementModel>();
            }
            return _movieConverterService.MoviesToMovieElements(_dbContext.Movies.OrderBy(x => x.Name)
                .Skip((currentPage - 1) * PaginationData.MaxItemsPerPage)
                .Take(PaginationData.MaxItemsPerPage).Include(x => x.Genres).Include(x => x.Reviews)
                .ToList()
                );
        }
    }
}
