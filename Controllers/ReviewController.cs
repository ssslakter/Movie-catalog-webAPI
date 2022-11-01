using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/movie")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("{movieId}/review/add"), Authorize]
        public async Task<IActionResult> AddReview([FromRoute] Guid movieId, ReviewModifyModel review)
        {
            var user = await _reviewService.GetUser(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            try
            {
                await _reviewService.AddReview(user, movieId, review);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Movie with id {movieId} was not found");
            }
            catch (ArgumentException)
            {
                return BadRequest("Review on this movie already exists");
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }

        }
        [HttpPut("{movieId}/review/{id}/edit"), Authorize]
        public async Task<IActionResult> EditReview([FromRoute] Guid movieId, [FromRoute] Guid id, ReviewModifyModel review)
        {
            var response = await FindReview(movieId, id);
            if (response.StatusCode != 200)
            {
                return response;
            }
            var user = await _reviewService.GetUser(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var rev = response.Value as Review;
            if (user.Id != rev?.AuthorData.Id)
            {
                return BadRequest("You're trying to edit review of another user");
            }

            await _reviewService.EditReview(rev, review);
            return Ok();
        }
        [HttpDelete("{movieId}/review/{id}/delete"), Authorize]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid movieId, [FromRoute] Guid id)
        {
            var response = await FindReview(movieId, id);
            if (response.StatusCode != 200)
            {
                return response;
            }
            var user = await _reviewService.GetUser(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var rev = response.Value as Review;
            if (user.Id != rev?.AuthorData.Id)
            {
                return BadRequest("You're trying to delete review of another user");
            }

            await _reviewService.DeleteReview(rev);
            return Ok();
        }

        private async Task<ObjectResult> FindReview(Guid movieId, Guid reviewId)
        {
            try
            {
                var review = await _reviewService.FindReview(movieId, reviewId);
                return Ok(review);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Movie with id {movieId} was not found");
            }
            catch (ArgumentException)
            {
                return NotFound($"Movie with id {movieId} does not have review with id {reviewId}");
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }
        }
    }
}
