using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;
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
        private readonly ITokenCacheService _tokenCacheService;

        public AuthController(IAuthService authService, ITokenCacheService tokenCacheService)
        {
            _authService = authService;
            _tokenCacheService = tokenCacheService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterModel model)
        {
            if (await _authService.IfUserExists(model))
            {
                return Problem(statusCode: 400, title: "DuplicateUserName",
                    detail: $"User with username \'{model.UserName}\' or email \'{model.Email}\' is already taken.");
            }
            try
            {
                await _authService.AddNewUserToDB(model);
                return await Login(new LoginCredentials { Username = model.UserName, Password = model.Password });
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong on the server");
            }
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
                return Problem(statusCode: 400, title: "Login failed", detail: " Incorrect username or password");
            }
            var jwt = _authService.CreateNewToken(identity);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Ok(new Dictionary<string, string>() { { "token", encodedJwt } });
        }


        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
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
                await _authService.Logout(Request.Headers[HeaderNames.Authorization]);
                return Ok(new Dictionary<string, string>() { { "token", "" }, { "message", "Logged Out" } });
            }
            catch
            {
                return Problem(statusCode: 500, title: "Something went wrong");
            }

        }
    }

}

