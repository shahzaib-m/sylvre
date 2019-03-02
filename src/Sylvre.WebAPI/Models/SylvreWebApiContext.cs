using Microsoft.EntityFrameworkCore;

namespace Sylvre.WebAPI.Models
{
    public class SylvreWebApiContext : DbContext
    {
        public SylvreWebApiContext(DbContextOptions<SylvreWebApiContext> options) : base(options) { }
    }
}
