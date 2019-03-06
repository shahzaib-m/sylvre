using Microsoft.EntityFrameworkCore;

namespace Sylvre.WebAPI.Entities
{
    public class SylvreWebApiContext : DbContext
    {
        public SylvreWebApiContext(DbContextOptions<SylvreWebApiContext> options) : base(options) { }
    }
}
