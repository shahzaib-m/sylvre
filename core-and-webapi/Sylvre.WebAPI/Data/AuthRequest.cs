using System.ComponentModel.DataAnnotations;

namespace Sylvre.WebAPI.Data
{
    public class AuthRequest
    {
        [Required]
        public string UsernameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
