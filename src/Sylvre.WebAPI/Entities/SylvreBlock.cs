using System.ComponentModel.DataAnnotations;

namespace Sylvre.WebAPI.Entities
{
    public class SylvreBlock
    {
        public int Id { get; set; }
        
        [MaxLength(50)]
        public string Name { get; set; }

        public string Body { get; set; }

        public int UserId { get; set; }
        public User User { get; }

        public bool IsSampleBlock { get; set; } = false;
    }
}
