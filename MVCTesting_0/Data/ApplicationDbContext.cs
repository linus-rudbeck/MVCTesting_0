using Microsoft.EntityFrameworkCore;
using MVCTesting_0.Models;

namespace MVCTesting_0.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
            
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public virtual DbSet<Song> Songs { get; set; }
    }
}
