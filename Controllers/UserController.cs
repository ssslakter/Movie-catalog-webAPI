using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MovieCatalogAPI.Exceptions;
using MovieCatalogAPI.Models.DTO;
using MovieCatalogAPI.Services;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService _userService;
        private readonly ITokenCacheService _tokenCacheService;

        public UserController(IUserService userService, ITokenCacheService tokenCacheService)
        {
            _tokenCacheService = tokenCacheService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Get()
        {
            if (await _tokenCacheService.IsTokenInDB(Request.Headers[HeaderNames.Authorization]))
            {
                return Unauthorized("Token is expired");
            }
            try
            {
                var profile = await _userService.GetUserProfile(User.Identity.Name);
                return Ok(profile);
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

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> Put(ProfileModel profile)
        {
            if (await _tokenCacheService.IsTokenInDB(Request.Headers[HeaderNames.Authorization]))
            {
                return Unauthorized("Token is expired");
            }
            try
            {
                await _userService.UpdateUserProfile(User.Identity.Name, profile);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }            
        }
    }
}
