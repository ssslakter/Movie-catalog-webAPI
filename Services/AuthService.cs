using MovieCatalogAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using MovieCatalogAPI.Configurations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;

namespace MovieCatalogAPI.Services
{
    public interface IAuthService
    {
        public void AddNewUserToDB(UserRegisterModel userRM);
        public bool IfUserExists(UserRegisterModel userRM);
        public ClaimsIdentity GetIdentity(string userName, string password);
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

        public bool IfUserExists(UserRegisterModel userRM)
        {
            return _dbContext.Users.Any(user => user.UserName == userRM.UserName && user.Email == userRM.Email);
        }

        public void AddNewUserToDB(UserRegisterModel userData)
        {
            var currUser = new User
            {
                UserName = userData.UserName,
                Name = userData.Name,
                Email = userData.Email,
                BirthDate = userData.BirthDate,
                Gender = userData.Gender
            };
            currUser.PasswordHash = _passwordHasher.HashPassword(currUser, userData.Password);
            _dbContext.Users.Add(currUser);
            _dbContext.SaveChanges();
        }

        public ClaimsIdentity GetIdentity(string userName, string password)
        {
            //potential error here
            var user = _dbContext.Users.Where(x => (x.UserName == userName)).AsEnumerable()
                .FirstOrDefault(x => _passwordHasher.VerifyHashedPassword(x, x.PasswordHash, password) != 0);
            if (user == null)
            {
                return null;
            }


            //Claims identity и будет являться полезной нагрузкой в JWT токене, которая будет проверяться стандартным атрибутом Authorize
            var claimsIdentity = new ClaimsIdentity("Token");
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
