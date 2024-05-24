using BlogWebApiDotNet.Models;

namespace BlogWebApiDotNet
{
    /// <summary>
    /// Class <c>Blog</c> models a blog object in the db.
    /// </summary>
    public partial class Blog
    {
        public long Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public DateTime Createdat { get; set; } = DateTime.UtcNow;

        public string? Userid { get; set; }

        public virtual AppUser? User { get; set; }

        public Blog() { }

        /// <summary>
        /// Static method <c>FromModel</c> is a utility helper to convert <c>BlogDTO</c> to Blog objects.
        ///  </summary>
        public static Blog FromModel(BlogDTO m_blogModel) =>
            new() { Body = m_blogModel.Body, Title = m_blogModel.Title };
    }
}
