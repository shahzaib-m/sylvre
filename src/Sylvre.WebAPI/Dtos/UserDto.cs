using System.ComponentModel.DataAnnotations;

namespace Sylvre.WebAPI.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }

        [MinLength(1)]
        [MaxLength(20)]
        public string Username { get; set; }

        [EmailAddress]
        [MinLength(3)]
        [MaxLength(254)] // https://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address
        public string Email { get; set; }

        [MaxLength(255)]
        public string FullName { get; set; }

        [MinLength(8)]
        public string Password { get; set; }
    }

    public class UserResponseDto
    {
        public int Id { get; set; }

        [MinLength(1)]
        [MaxLength(20)]
        public string Username { get; set; }

        [EmailAddress]
        [MinLength(3)]
        [MaxLength(254)] // https://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address
        public string Email { get; set; }

        [MaxLength(255)]
        public string FullName { get; set; }
    }
}

