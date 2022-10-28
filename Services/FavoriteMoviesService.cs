using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Models;
using System.ComponentModel;

namespace MovieCatalogAPI.Services
{
    public interface IFavoriteMoviesService
    {
        Task<MoviesList> GetMovies(string userName);
        Task AddMovie(string userName, Guid movieId);
        Task RemoveMovie(string userName, Guid movieId);
        Task<bool> IfMovieExists(Guid movieId);
        Task<bool> IfUserHasMovie(string userName, Guid movieId);
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

        async Task IFavoriteMoviesService.AddMovie(string userName, Guid movieId)
        {
            var user = await GetUser(userName);
            var movie = await _dbContext.Movies.FirstOrDefaultAsync(x => x.Id == movieId);
            user.FavoriteMovies.Add(movie);
            _logger.LogInformation("Movie was succesefully added to favorite");
            await _dbContext.SaveChangesAsync();
        }

        async Task<MoviesList> IFavoriteMoviesService.GetMovies(string userName)
        {
            var user = await GetUser(userName);
            if (user == null)
            {
                throw new NullReferenceException();
            }
            if (user.FavoriteMovies == null)
            {
                _logger.LogWarning($"movie list of the user {userName} is Null. Returning Empty list element");
                return new MoviesList();
            }
            var movieList = new MoviesList { Movies = _movieConverterService.MoviesToMovieElements(user.FavoriteMovies.ToList()) };
            _logger.LogInformation("movie list was succesfully found in DB and converted to DTO");
            return movieList;
        }

        async Task IFavoriteMoviesService.RemoveMovie(string userName, Guid movieId)
        {
            var user = await GetUser(userName);
            if (user == null)
            {
                throw new NullReferenceException();
            }
            var movie = await _dbContext.Movies.FirstOrDefaultAsync(x => x.Id == movieId);
            user.FavoriteMovies.Remove(movie);
            await _dbContext.SaveChangesAsync();
        }


        private async Task<User?> GetUser(string userName)
        {
            var user = await _dbContext.Users.Include(x => x.FavoriteMovies).ThenInclude(x=>x.Genres)
                .Include(x=>x.FavoriteMovies).ThenInclude(m=>m.Reviews).FirstOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
            {
                _logger.LogWarning($"user with username {userName} wasn't found");
            }
            return user;
        }

        async Task<bool> IFavoriteMoviesService.IfMovieExists(Guid movieId)
        {
            return await _dbContext.Movies.AnyAsync(x => x.Id == movieId);
        }

        async Task<bool> IFavoriteMoviesService.IfUserHasMovie(string userName, Guid movieId)
        {
            var user = await GetUser(userName);
            return user.FavoriteMovies.Any(x => x.Id == movieId);
        }
    }
}
