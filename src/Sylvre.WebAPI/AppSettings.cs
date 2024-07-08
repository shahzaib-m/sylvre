namespace Sylvre.WebAPI
{
    public class AppSettings
    {
        public string SylApi_Secret { get; set; }
        public string SylApi_KnownProxies { get; set; }

        public string SylApi_DbServer { get; set; }
        public string SylApi_DbPort { get; set; }
        public string SylApi_DbName { get; set; }
        public string SylApi_DbUser { get; set; }
        public string SylApi_DbPassword { get; set; }
    }
}
