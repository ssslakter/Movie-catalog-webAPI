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
                PageSize = movies.Count()
            };
            return Ok(new { pageInfo, movies });
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetDetails([FromRoute] Guid id)
        {
            try
            {
                var details = await _movieInfoService.GetMovieDetails(id);
                return Ok(details);
            }
            catch(KeyNotFoundException)
            {
                return NotFound("Movie with this id does not exist");
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }

        }
    }
}
