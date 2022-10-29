using Microsoft.AspNetCore.Mvc;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/review")]
    [ApiController]
    public class ReviewController : Controller
    {
        [HttpPost("{movieId}/review/add")]
        public IActionResult AddReview([FromRoute] Guid movieId)
        {
            throw new NotImplementedException();
        }
        [HttpPut("{movieId}/review/edit")]
        public IActionResult EditReview([FromRoute] Guid movieId)
        {
            throw new NotImplementedException();
        }
        [HttpDelete("{movieId}/review/delete")]
        public IActionResult DeleteReview([FromRoute] Guid movieId)
        {
            throw new NotImplementedException();
        }
    }
}
