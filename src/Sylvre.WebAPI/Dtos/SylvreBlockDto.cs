using System.ComponentModel.DataAnnotations;

namespace Sylvre.WebAPI.Dtos
{
    public class SylvreBlockDto
    {
        [MaxLength(50)]
        public string Name { get; set; }

        public string Body { get; set; }
    }

    public class SylvreBlockResponseDto
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public string Body { get; set; }
    }
}
