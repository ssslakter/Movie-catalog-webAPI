using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;
using System.Data;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "admin")]
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
            try
            {
                return Ok(await _movieDataService.GetMovies(page));
            }
            catch
            {
                return BadRequest("Incorrect request format");
            }
        }
    }
}
