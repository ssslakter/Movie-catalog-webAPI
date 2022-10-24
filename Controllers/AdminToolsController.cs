using Microsoft.AspNetCore.Mvc;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;

namespace MovieCatalogAPI.Controllers
{
    public class AdminToolsController : Controller
    {
        private IMovieDataService _getMovieDataService;

        public AdminToolsController(IMovieDataService getMovieDataService)
        {
            _getMovieDataService = getMovieDataService;
        }

        [HttpPost]
        public IActionResult Post(MovieElementModel movie)
        {
            _getMovieDataService.AddMovieToDB(movie);
            return Ok();
        }
    }
}
