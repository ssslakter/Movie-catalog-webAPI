using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.DTO;
using System.ComponentModel;

namespace MovieCatalogAPI.Services
{
    public interface IUserService
    {
        public Task<ProfileModel> GetUserProfile(string userName);
        public Task<bool> IsEmailTaken(string email);
        public Task UpdateUserProfile(ProfileModel profile);
    }
    public class UserService : IUserService
    {
        private readonly MovieDBContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(MovieDBContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ProfileModel> GetUserProfile(string userName)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
            {
                _logger.LogInformation("User was not found in DB");
                throw new ArgumentNullException(nameof(userName));
            }
            return new ProfileModel(user.Email, user.Name, user.UserName)
            {
                Id = user.Id,
                AvatarLink = user.AvatarLink,
                BirthDate = user.BirthDate,
                Gender = user.Gender
            };
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return await _dbContext.Users.AnyAsync(x => x.Email == email);
        }

        public async Task UpdateUserProfile(ProfileModel profile)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == profile.Id);
            if (user == null)
            {
                _logger.LogInformation("User was not found in DB");
                throw new InvalidEnumArgumentException($"User with ID {profile.Id} doesn't exist!");
            }
            user.BirthDate = profile.BirthDate;
            user.AvatarLink = profile.AvatarLink;
            user.Name = profile.Name;
            user.Gender = profile.Gender;
            if (user.Email == profile.Email || (await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == profile.Email)) == null)
            {
                user.Email = profile.Email;
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Succesfully changed profile");
        }
    }
}
