using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/favorites")]
    [ApiController]
    public class FavoriteMoviesController : ControllerBase
    {
        [HttpGet, Authorize]
        public async Task<MoviesList> Get()
        {
            throw new NotImplementedException();
        }
        [HttpPost("{id}/add"), Authorize]
        public async Task<IActionResult> AddMovie()
        {
            throw new NotImplementedException();
        }
        [HttpDelete("{id}/delete"), Authorize]
        public async Task<IActionResult> DeleteMovie()
        {
            throw new NotImplementedException();
        }
    }
}
