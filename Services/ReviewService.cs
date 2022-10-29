using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Services
{
    public interface IReviewService
    {
        Task<User?> GetUser(string? userName);
        Task AddReview(User user, Guid movieId, ReviewModifyModel reviewModel);
        Task EditReview(User user, Guid movieId, ReviewModifyModel reviewModel);
        Task DeleteReview(User user, Guid movieId);
    }
    public class ReviewService : IReviewService
    {
        private readonly MovieDBContext _dbContext;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(MovieDBContext dbContext, ILogger<ReviewService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        async Task IReviewService.AddReview(User user, Guid movieId, ReviewModifyModel reviewModel)
        {
            var movie = await _dbContext.Movies.Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == movieId);
            if (movie == null)
            {
                _logger.LogWarning($"Film with id {movieId} seem does not exist");
                throw new KeyNotFoundException();
            }
            if (user.Reviews.Any(x => x.Movie.Id == movieId))
            {
                _logger.LogInformation("User tried to add review that already exits");
                throw new ArgumentException();
            }
            var review = new Review
            {
                Movie = movie,
                ReviewText = reviewModel.ReviewText,
                Rating = reviewModel.Rating,
                IsAnonymous = reviewModel.IsAnonymous,
                CreateDateTime = DateTime.UtcNow,
                AuthorData = user,
                Author = new UserShort
                {
                    UserID = user.Id,
                    Avatar = user.AvatarLink,
                    UserName = user.UserName
                }
            };
            movie.Reviews.Add(review);
            user.Reviews.Add(review);
            await _dbContext.SaveChangesAsync();
        }

        Task IReviewService.DeleteReview(User user, Guid movieId)
        {
            throw new NotImplementedException();
        }

        Task IReviewService.EditReview(User user, Guid movieId, ReviewModifyModel reviewModel)
        {
            throw new NotImplementedException();
        }

        async Task<User?> IReviewService.GetUser(string? userName)
        {
            return await _dbContext.Users.Include(x => x.Reviews).FirstOrDefaultAsync(x => x.UserName == userName);
        }
    }
}
