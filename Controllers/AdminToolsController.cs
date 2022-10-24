using Microsoft.AspNetCore.Mvc;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminToolsController : Controller
    {
        private IMovieDataService _movieDataService;

        public AdminToolsController(IMovieDataService getMovieDataService)
        {
            _movieDataService = getMovieDataService;
        }

        [HttpPost("addFilms")]
        public async Task<IActionResult> Post(int page)
        {
            await _movieDataService.AddMoviesPageToDB(page);
            return Ok();
        }

        [HttpGet("getFilms")]
        public async Task<IActionResult> Get(int page)
        {
            return Ok(await _movieDataService.GetMovies(page));
        }
    }
}
