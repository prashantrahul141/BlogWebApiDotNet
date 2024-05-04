using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApiDotNet.Models {
    public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<AppUser>(options) {
        public virtual DbSet<Blog> Blogs { get; set; }
    }

}