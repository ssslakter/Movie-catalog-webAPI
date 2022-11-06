using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MovieCatalogAPI.Exceptions;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/movie")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly ITokenCacheService _tokenCacheService;

        public ReviewController(IReviewService reviewService, ITokenCacheService tokenCacheService)
        {
            _tokenCacheService = tokenCacheService;
            _reviewService = reviewService;
        }

        [HttpPost("{movieId}/review/add"), Authorize]
        public async Task<IActionResult> AddReview([FromRoute] Guid movieId, ReviewModifyModel review)
        {
            try
            {
                if (await _tokenCacheService.IsTokenInDB(Request.Headers[HeaderNames.Authorization]))
                {
                    return Unauthorized("Token is expired");
                }
            }
            catch { }           
            
            try
            {
                await _reviewService.AddReview(User.Identity.Name, movieId, review);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }

        }
        [HttpPut("{movieId}/review/{id}/edit"), Authorize]
        public async Task<IActionResult> EditReview([FromRoute] Guid movieId, [FromRoute] Guid id, ReviewModifyModel review)
        {
            try
            {
                if (await _tokenCacheService.IsTokenInDB(Request.Headers[HeaderNames.Authorization]))
                {
                    return Unauthorized("Token is expired");
                }
            }
            catch { }
            try
            {
                var rev = await _reviewService.FindReview(movieId, id);
                await _reviewService.EditReview(User.Identity.Name, rev, review);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionDeniedExeption e)
            {
                return BadRequest(e.Message);
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }
        }
        [HttpDelete("{movieId}/review/{id}/delete"), Authorize]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid movieId, [FromRoute] Guid id)
        {
            try
            {
                if (await _tokenCacheService.IsTokenInDB(Request.Headers[HeaderNames.Authorization]))
                {
                    return Unauthorized("Token is expired");
                }
            }
            catch { }
            try
            {
                var rev = await _reviewService.FindReview(movieId, id);
                await _reviewService.DeleteReview(User.Identity.Name, rev);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionDeniedExeption e)
            {
                return BadRequest(e.Message);
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }
        }
    }
}
