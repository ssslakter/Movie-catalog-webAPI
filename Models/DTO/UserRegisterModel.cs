using System.ComponentModel.DataAnnotations;

namespace MovieCatalogAPI.Models
{

    public class UserRegisterModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
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
