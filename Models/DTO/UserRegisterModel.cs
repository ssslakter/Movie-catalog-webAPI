using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MovieCatalogAPI.Models
{

    public class UserRegisterModel
    {
        [Required,PasswordPropertyText]
        public string UserName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, PasswordPropertyText,MinLength(6)]
        public string Password { get; set; }
        [Required,EmailAddress]
        public string Email { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public Gender? Gender { get; set; }

        public UserRegisterModel(string userName, string name, string email, string password)
        {
            Name = name;
            UserName = userName;
            Email = email;
            Password = password;
        }
    }
}
