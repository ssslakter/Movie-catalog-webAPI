using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MovieCatalogAPI.Exceptions;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.Core_data;
using MovieCatalogAPI.Models.DTO;
using MovieCatalogAPI.Services;
using System.Data;
using System.Text.Json.Nodes;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminToolsController : Controller
    {
        private IMovieDataService _movieDataService;
        private ITokenCacheService _tokenCacheService;

        public AdminToolsController(IMovieDataService getMovieDataService, ITokenCacheService tokenCacheService)
        {
            _movieDataService = getMovieDataService;
            _tokenCacheService = tokenCacheService;
        }

        [HttpPost("addFilmsFromKreosoft/{page}")]
        public async Task<IActionResult> Post([FromRoute] int page)
        {
            try
            {
                if (await _tokenCacheService.IsTokenInDB(Request.Headers[HeaderNames.Authorization]))
                {
                    return Unauthorized("Token is expired");
                }
            }
            catch { }
            await _movieDataService.AddMoviesPageToDB(page);
            return Ok();
        }

        [HttpGet("getFilmsFromKreosoft/{page}")]
        public async Task<ActionResult<MoviesPagedListModel>> Get([FromRoute] int page)
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
                return Ok(await _movieDataService.GetMovies(page));
            }
            catch
            {
                return BadRequest("Incorrect request format");
            }
        }

        [HttpPost("addMovie")]
        public async Task<IActionResult> AddFilm(MovieInsertModel movieDetails)
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
                await _movieDataService.AddMovie(movieDetails);
                return Ok();
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }

        }

        [HttpPut("{movieId}/edit")]
        public async Task<IActionResult> EditMovie([FromRoute] Guid movieId, MovieInsertModel movieDetails)
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
                await _movieDataService.EditMovie(movieId, movieDetails);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }
        }

        [HttpDelete("{movieId}/delete")]
        public async Task<IActionResult> DeleteMovie(Guid movieId)
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
                await _movieDataService.DeleteMovie(movieId);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }

        }

        [HttpGet("getGenres")]
        public ActionResult<IEnumerable<GenreModel>> GetGenrese()
        {
            try
            {
                return Ok(new Dictionary<string, IEnumerable<GenreModel>> { { "genres", _movieDataService.GetGenres() } });
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }
        }
    }
}
