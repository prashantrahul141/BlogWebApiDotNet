using BlogWebApiDotNet.Models;

namespace BlogWebApiDotNet {
    public partial class Blog {
        public long Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public DateTime Createdat { get; set; }

        public string? Userid { get; set; }

        public virtual AppUser? User { get; set; }

        public Blog() { }

        public static Blog FromModel(BlogDTO m_blogModel) => new() { Body = m_blogModel.Body, Title = m_blogModel.Title };

    }
}