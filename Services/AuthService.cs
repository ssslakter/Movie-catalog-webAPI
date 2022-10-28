using MovieCatalogAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using MovieCatalogAPI.Configurations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MovieCatalogAPI.Services
{
    public interface IAuthService
    {
        public Task AddNewUserToDB(UserRegisterModel userRM);
        public Task<bool> IfUserExists(UserRegisterModel userRM);
        public Task<ClaimsIdentity> GetIdentity(string userName, string password);
        public JwtSecurityToken CreateNewToken(ClaimsIdentity identity);
    }
    public class AuthService : IAuthService
    {
        private readonly MovieDBContext _dbContext;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(MovieDBContext dbContext)
        {
            _passwordHasher = new PasswordHasher<User>();
            _dbContext = dbContext;
        }

        public async Task<bool> IfUserExists(UserRegisterModel userRM)
        {
            return await _dbContext.Users.AnyAsync(user => user.UserName == userRM.UserName || user.Email == userRM.Email);
        }

        public async Task AddNewUserToDB(UserRegisterModel userData)
        {
            var currUser = new User
            {
                UserName = userData.UserName,
                Name = userData.Name,
                Email = userData.Email,
                BirthDate = userData.BirthDate.ToUniversalTime(),
                Gender = userData.Gender,
                Role="user"
            };
            currUser.PasswordHash = _passwordHasher.HashPassword(currUser, userData.Password);
            await _dbContext.Users.AddAsync(currUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ClaimsIdentity> GetIdentity(string userName, string password)
        {
            //potential error here
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => (x.UserName == userName));

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == 0)
            {
                return null;
            }
            var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            new Claim(ClaimsIdentity.DefaultRoleClaimType,user.Role)
        };

            //Claims identity и будет являться полезной нагрузкой в JWT токене, которая будет проверяться стандартным атрибутом Authorize
            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;

        }

        public JwtSecurityToken CreateNewToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: JwtConfigurations.Issuer,
                audience: JwtConfigurations.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.AddMinutes(JwtConfigurations.Lifetime),
                signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return jwt;
        }
    }
}
