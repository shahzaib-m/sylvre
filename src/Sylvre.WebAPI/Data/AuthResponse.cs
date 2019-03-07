namespace Sylvre.WebAPI.Data
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
