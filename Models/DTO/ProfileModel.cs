using System.ComponentModel.DataAnnotations;

namespace MovieCatalogAPI.Models.DTO
{
    public class ProfileModel
    {
        public Guid Id { get; set; }
        public string? NickName { get; set; }
        [EmailAddress,Required]
        public string Email { get; set; }
        public string? AvatarLink { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [EnumDataType(typeof (Gender))]
        public Gender? Gender { get; set; }
        public string Role { get; set; }
    }
}
