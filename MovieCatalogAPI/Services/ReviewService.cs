using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using MovieCatalogAPI.Exceptions;
using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Services
{
    public interface IReviewService
    {
        Task AddReview(string? userName, Guid movieId, ReviewModifyModel reviewModel);
        Task EditReview(string? userName, Review review, ReviewModifyModel reviewModel);
        Task DeleteReview(string? userName, Review review);
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

        async Task IReviewService.AddReview(string? userName, Guid movieId, ReviewModifyModel reviewModel)
        {
            var user = await GetUser(userName);
            if (user == null)
            {
                throw new NotFoundException("Current user not found");
            }
            var movie = await _dbContext.Movies.Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == movieId);
            if (movie == null)
            {
                _logger.LogInformation($"Movie with id {movieId} seem does not exist");
                throw new NotFoundException($"Movie with id {movieId} was not found");
            }
            if (user.Reviews.Any(x => x.Movie.Id == movieId))
            {
                _logger.LogInformation("User tried to add review that already exists");
                throw new ArgumentException("Review on this movie already exists");
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

        async Task IReviewService.DeleteReview(string? userName, Review review)
        {
            var user = await GetUser(userName);
            if (user == null)
            {
                _logger.LogInformation($"User with username {userName} was not found");
                throw new NotFoundException("Current user not found");
            }
            if (user.Id != review.AuthorData.Id && user.Role!="admin")
            {
                _logger.LogInformation($"User {userName} tried to delete review of user {review.AuthorData.UserName}");
                throw new PermissionDeniedExeption("You're trying to edit review of another user");
            }
            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync();
        }

        async Task IReviewService.EditReview(string? userName, Review review, ReviewModifyModel reviewModel)
        {
            var user = await GetUser(userName);
            if (user == null)
            {
                _logger.LogInformation($"User with username {userName} was not found");
                throw new NotFoundException("Current user not found");
            }
            if (user.Id != review.AuthorData.Id)
            {
                _logger.LogInformation($"User {userName} tried to edit review of user {review.AuthorData.UserName}");
                throw new PermissionDeniedExeption("You're trying to edit review of another user");
            }
            review.ReviewText = reviewModel.ReviewText;
            review.Rating = reviewModel.Rating;
            review.IsAnonymous = reviewModel.IsAnonymous;
            await _dbContext.SaveChangesAsync();
        }

        private async Task<User?> GetUser(string? userName)
        {
            return await _dbContext.Users.Include(x => x.Reviews)?.ThenInclude(x => x.Movie).FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<Review> FindReview(Guid movieId, Guid reviewId)
        {
            var movie = await _dbContext.Movies.Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == movieId);
            if (movie == null)
            {
                _logger.LogInformation($"Movie with id {movieId} seem does not exist");
                throw new NotFoundException($"Movie with id {movieId} was not found");
            }
            var review = await _dbContext.Reviews.Include(x => x.Movie).Include(x => x.AuthorData).FirstOrDefaultAsync(x => x.Id == reviewId);
            if (review == null || review.Movie.Id != movieId)
            {
                _logger.LogInformation($"Movie with id {movieId} does not have review with id {reviewId}");
                throw new NotFoundException($"Movie with id {movieId} does not have review with id {reviewId}");
            }
            return review;
        }
    }
}
