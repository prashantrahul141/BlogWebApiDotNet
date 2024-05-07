using Microsoft.AspNetCore.Identity;

namespace BlogWebApiDotNet
{
    public partial class AppUser : IdentityUser
    {
        public string Image { get; set; } = "https://avatars.githubusercontent.com/u/59825803?v=4";
        public virtual ICollection<Blog> Blogs { get; set; } = [];

        public AppUser() { }
    }
}
