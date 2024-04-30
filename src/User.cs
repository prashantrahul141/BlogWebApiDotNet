using Microsoft.AspNetCore.Identity;

namespace BlogWebApiDotNet {
    public partial class User : IdentityUser {

        public string Image { get; set; } = string.Empty;
        public virtual ICollection<Blog> Blogs { get; set; } = [];

        public User() { }

    }
}