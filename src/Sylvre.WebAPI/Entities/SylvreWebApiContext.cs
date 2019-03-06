using Microsoft.EntityFrameworkCore;
using Sylvre.WebAPI.Dtos;

namespace Sylvre.WebAPI.Entities
{
    public class SylvreWebApiContext : DbContext
    {
        public SylvreWebApiContext(DbContextOptions<SylvreWebApiContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Sets the unique columns in the tables.
        /// https://stackoverflow.com/questions/41246614/entity-framework-core-add-unique-constraint-code-first
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                   .HasIndex(user => new { user.Username, user.Email })
                   .IsUnique(true);
        }
    }
}
