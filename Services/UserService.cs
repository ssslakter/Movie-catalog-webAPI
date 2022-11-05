using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Exceptions;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.DTO;
using System.ComponentModel;

namespace MovieCatalogAPI.Services
{
    public interface IUserService
    {
        public Task<ProfileModel> GetUserProfile(string userName);
        public Task UpdateUserProfile(string userName, ProfileModel profile);
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
            var user = await FindUserByUserName(userName);
            return new ProfileModel(user.Email, user.Name, user.UserName)
            {
                Id = user.Id,
                AvatarLink = user.AvatarLink,
                BirthDate = user.BirthDate,
                Gender = user.Gender
            };
        }


        public async Task UpdateUserProfile(string userName, ProfileModel profile)
        {
            var user = await FindUserByUserName(userName);
            if (user.UserName != profile.UserName)
            {
                _logger.LogInformation("User tried to change his userName");
                throw new PermissionDeniedExeption("You are not allowed to change userName");
            }
            if(user.Id != profile.Id)
            {
                _logger.LogInformation("User tried to change his userName");
                throw new PermissionDeniedExeption("You are only allowed to change your profile");
            }
            user.BirthDate = profile.BirthDate.ToUniversalTime();
            user.AvatarLink = profile.AvatarLink;
            user.Name = profile.Name;
            user.Gender = profile.Gender;
            if (user.Email == profile.Email || !await IsEmailTaken(profile.Email))
            {
                user.Email = profile.Email;
            }
            else
            {
                _logger.LogInformation("User tried to set already used email");
                throw new PermissionDeniedExeption($"Email {profile.Email} is already taken");
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Succesfully changed profile");
        }

        private async Task<bool> IsEmailTaken(string email)
        {
            return await _dbContext.Users.AnyAsync(x => x.Email == email);
        }

        private async Task<User> FindUserByUserName(string name)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == name);
            if (user == null)
            {
                _logger.LogInformation("User was not found in DB");
                throw new NotFoundException($"User with ID {name} doesn't exist!");
            }
            return user;
        }
    }
}
