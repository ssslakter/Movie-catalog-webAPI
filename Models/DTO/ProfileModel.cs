using System.ComponentModel.DataAnnotations;

namespace MovieCatalogAPI.Models.DTO
{
    public class ProfileModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? AvatarLink { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public Gender? Gender { get; set; }

        public ProfileModel(string email, string name,string userName)
        {
            Email = email;
            Name = name;          
            UserName = userName;
        }
    }
}
