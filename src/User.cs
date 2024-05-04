using Microsoft.AspNetCore.Identity;

namespace BlogWebApiDotNet {
    public partial class AppUser : IdentityUser {
        public string Image { get; set; } = string.Empty;
        public virtual ICollection<Blog> Blogs { get; set; } = [];

        public AppUser() {
        }

    }
}