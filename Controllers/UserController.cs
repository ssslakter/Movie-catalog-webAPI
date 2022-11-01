using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalogAPI.Models.DTO;
using MovieCatalogAPI.Services;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Get()
        {
            var profile = await _userService.GetUserProfile(User.Identity.Name);
            return Ok(profile);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> Put(ProfileModel profile)
        {
            var current = await _userService.GetUserProfile(User.Identity.Name);
            if (profile.UserName != current.UserName)
            {
                return BadRequest("You are not allowed to change userName");
            }
            if (profile.Email != current.Email && await _userService.IsEmailTaken(profile.Email))
            {
                return BadRequest($"Email {profile.Email} is already taken");
            }
            await _userService.UpdateUserProfile(profile);

            return Ok();
        }
    }
}
