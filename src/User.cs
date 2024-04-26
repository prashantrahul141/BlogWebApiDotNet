using Microsoft.AspNetCore.Identity;

namespace BlogWebApiDotNet {
    public partial class User : IdentityUser {

        public virtual ICollection<Blog> Blogs { get; set; } = [];

        public User() { }



    }
}