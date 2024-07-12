using System.ComponentModel.DataAnnotations;

namespace Sylvre.WebAPI.Data
{
    public class ChangePasswordRequest
    {
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}
