using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sylvre.WebAPI.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MinLength(3)]
        [MaxLength(254)] // https://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address
        public string Email { get; set; }

        [MaxLength(255)]
        public string FullName { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
