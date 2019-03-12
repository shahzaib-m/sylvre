namespace Sylvre.WebAPI
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string KnownProxies { get; set; }
        public string CorsOrigin { get; set; }
        public string CookieDomain { get; set; }
    }
}
