using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;
using System.IdentityModel.Tokens.Jwt;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterModel model)
        {
            if (await _authService.IfUserExists(model))
            {
                return BadRequest(new { errorText = $"Username \'{model.UserName}\' is already taken." });
            }
            await _authService.AddNewUserToDB(model);
            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCredentials model)
        {
            if (model.Username == null || model.Password == null)
            {
                return BadRequest(new { errorText = "Username or password where null." });
            }
            var identity = await _authService.GetIdentity(model.Username, model.Password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Login failed. Incorrect username or password" });
            }
            var jwt = _authService.CreateNewToken(identity);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new JsonResult(new Dictionary<string, string>() { { "token", encodedJwt } });
        }

        [Authorize]
        [HttpGet("check")]
        public IActionResult CheckAuthor()
        {
            return Ok($"Authorized {User.Identity.Name}");

        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            //Возможно костыль
            Response.Headers.Remove("Authorization");
            return new JsonResult(new Dictionary<string, string>() { { "token", "" }, { "message", "Logged Out" } });
        }
    }

}

