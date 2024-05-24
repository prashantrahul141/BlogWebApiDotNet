using Microsoft.AspNetCore.Identity;

namespace BlogWebApiDotNet
{
    /// <summary>
    /// Overloads the builin IdentityUser to add new fields.
    /// </summary>
    public partial class AppUser : IdentityUser
    {
        // User avatar.
        public string Image { get; set; } = "https://avatars.githubusercontent.com/u/59825803?v=4";

        // collection of blogs they created.
        public virtual ICollection<Blog> Blogs { get; set; } = [];

        public AppUser() { }
    }
}
