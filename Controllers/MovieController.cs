using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;

namespace MovieCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MovieController : Controller
    {
        private readonly IMovieInfoService _movieInfoService;

        public MovieController(IMovieInfoService movieInfoService)
        {
            _movieInfoService = movieInfoService;
        }

        //[HttpGet("{page}")]
        [HttpGet]
        public ActionResult<MovieElement[]> Get([FromRoute] int page)
        {
            var movies = _movieInfoService.GetMovieElements();
            return Ok(movies);
        }
        [HttpPost]
        public ActionResult Post(MovieElement movie)
        {
            _movieInfoService.WriteToDB(movie);
            return Ok();
        }
    }
}
