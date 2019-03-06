using System.ComponentModel.DataAnnotations;

namespace Sylvre.WebAPI.Data
{
    public class UserAuthRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
