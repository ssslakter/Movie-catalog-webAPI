using System.ComponentModel.DataAnnotations;

namespace MovieCatalogAPI.Models
{
    public class UserShort
    {
        public Guid UserID { get; set; }
        public string? UserName { get; set; }
        public string? Avatar { get; set; }
    }
}
