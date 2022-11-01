using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Services
{
    public interface IReviewService
    {
        Task<User?> GetUser(string? userName);
        Task AddReview(User user, Guid movieId, ReviewModifyModel reviewModel);
        Task EditReview(Review review, ReviewModifyModel reviewModel);
        Task DeleteReview(Review review);
        Task<Review> FindReview(Guid movieId, Guid reviewId);
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
                _logger.LogInformation($"Movie with id {movieId} seem does not exist");
                throw new KeyNotFoundException();
            }
            if (user.Reviews.Any(x => x.Movie.Id == movieId))
            {
                _logger.LogInformation("User tried to add review that already exists");
                throw new ArgumentException();
            }
            var review = new Review
            {
                Movie = movie,
                ReviewText = reviewModel.ReviewText,
                Rating = reviewModel.Rating,
                IsAnonymous = reviewModel.IsAnonymous,
                CreateDateTime = DateTime.UtcNow,
                AuthorData = user
            };
            movie.Reviews.Add(review);
            user.Reviews.Add(review);
            await _dbContext.SaveChangesAsync();
        }

        async Task IReviewService.DeleteReview(Review review)
        {
            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync();
        }

        async Task IReviewService.EditReview(Review review, ReviewModifyModel reviewModel)
        {
            review.ReviewText = reviewModel.ReviewText;
            review.Rating = reviewModel.Rating;
            review.IsAnonymous = reviewModel.IsAnonymous;
            await _dbContext.SaveChangesAsync();
        }

        async Task<User?> IReviewService.GetUser(string? userName)
        {
            return await _dbContext.Users.Include(x => x.Reviews)?.ThenInclude(x => x.Movie).FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<Review> FindReview(Guid movieId, Guid reviewId)
        {
            var movie = await _dbContext.Movies.Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == movieId);
            if (movie == null)
            {
                _logger.LogInformation($"Movie with id {movieId} seem does not exist");
                throw new KeyNotFoundException();
            }
            var review = await _dbContext.Reviews.Include(x => x.Movie).Include(x => x.AuthorData).FirstOrDefaultAsync(x => x.Id == reviewId);
            if (review == null || review.Movie.Id != movieId)
            {
                _logger.LogInformation($"Movie with id {movieId} does not have review with id {reviewId}");
                throw new ArgumentException();
            }
            return review;
        }
    }
}
