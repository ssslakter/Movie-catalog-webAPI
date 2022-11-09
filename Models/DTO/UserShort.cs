using System.ComponentModel.DataAnnotations;

namespace MovieCatalogAPI.Models
{
    public class UserShort
    {
        public Guid UserId { get; set; }
        public string? NickName { get; set; }
        public string? Avatar { get; set; }
    }
}
