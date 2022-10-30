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
        [HttpPut("{movieId}/review/edit"), Authorize]
        public IActionResult EditReview([FromRoute] Guid movieId)
        {
            throw new NotImplementedException();
        }
        [HttpDelete("{movieId}/review/{id}/delete"), Authorize]
        public IActionResult DeleteReview([FromRoute] Guid movieId, [FromRoute] Guid reviewId)
        {
            throw new NotImplementedException();
        }
    }
}
