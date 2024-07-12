using System;

namespace Sylvre.WebAPI.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string TokenIdHash { get; set; }
        public DateTime ExpiryUtc { get; set; }

        public string UserAgent { get; set; }
        public string IpAddress { get; set; }

        public int UserId { get; set; }
        public User User { get; }
    }
}
