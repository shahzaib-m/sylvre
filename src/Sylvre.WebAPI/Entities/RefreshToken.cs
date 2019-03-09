namespace Sylvre.WebAPI.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Signature { get; set; }
        public bool IsExpired { get; set; }

        public string UserAgent { get; set; }
        public string IpAddress { get; set; }

        public int UserId { get; set; }
        public User User { get; }
    }
}
