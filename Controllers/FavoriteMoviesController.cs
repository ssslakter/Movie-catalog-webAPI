using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/favorites")]
    [ApiController]
    public class FavoriteMoviesController : ControllerBase
    {
        private readonly IFavoriteMoviesService _favoriteMoviesService;

        public FavoriteMoviesController(IFavoriteMoviesService favoriteMoviesService)
        {
            _favoriteMoviesService = favoriteMoviesService;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _favoriteMoviesService.GetMovies(User.Identity.Name));
            }
            catch
            {
                return Problem(statusCode:500, title: "Something went wrong");
            }
           
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
