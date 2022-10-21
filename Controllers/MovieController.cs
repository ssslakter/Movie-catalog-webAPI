using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieCatalogAPI.Configurations;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.DTO;
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

        [HttpGet("{page}")]
        public IActionResult Get([FromRoute] int page)
        {
            var correctPage = Math.Max(1, Math.Min(page, PaginationData.TotalPageCount));
            var movies = _movieInfoService.GetMovieElements(correctPage);

            var pageInfo = new PageInfoModel
            {
                CurrentPage = correctPage,
                PageCount = PaginationData.TotalPageCount,
                PageSize = movies.Count
            };
            return Ok(new { pageInfo, movies });
        }
        [HttpPost]
        public IActionResult Post(MovieElementModel movie)
        {
            _movieInfoService.WriteToDB(movie);
            return Ok();
        }
    }
}
