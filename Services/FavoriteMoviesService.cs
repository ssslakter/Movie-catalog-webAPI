using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Services
{
    public interface IFavoriteMoviesService
    {
        Task<MoviesList> GetMovies(string? userName);
        Task AddMovie(Guid id);
        Task RemoveMovie(Guid id);
    }
    public class FavoriteMoviesService : IFavoriteMoviesService
    {
        private readonly MovieDBContext _dbContext;
        private readonly IMovieConverterService _movieConverterService;
        private readonly ILogger<FavoriteMoviesService> _logger;

        public FavoriteMoviesService(MovieDBContext dbContext, ILogger<FavoriteMoviesService> logger, IMovieConverterService movieConverterService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _movieConverterService = movieConverterService;
        }

        Task IFavoriteMoviesService.AddMovie(Guid id)
        {
            throw new NotImplementedException();
        }

        async Task<MoviesList> IFavoriteMoviesService.GetMovies(string? userName)
        {
            var user = await _dbContext.Users.Include(x => x.FavoriteMovies).FirstOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
            {
                _logger.LogWarning($"user with username {userName} wasn't found");
                throw new ArgumentNullException(nameof(userName));
            }
            try
            {
                var movieList= new MoviesList { Movies = _movieConverterService.MoviesToMovieElements(user.FavoriteMovies?.ToList()) };
                _logger.LogInformation("movie list was succesfully found in DB and converted to DTO");
                return movieList;
            }
            catch(NullReferenceException)
            {
                _logger.LogWarning($"movie list of the user {userName} is Null. Returning Null...");
                return null;
            }
            catch
            {
                _logger.LogError("Something went wrong");
                throw;
            }
        }

        Task IFavoriteMoviesService.RemoveMovie(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
