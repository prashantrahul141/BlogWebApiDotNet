namespace BlogWebApiDotNet.Models {
    public class BlogDTO {
        public required string Title { get; set; }
        public required string Body { get; set; }

    }

    public class BlogDTOReturn {
        public required string UserId { get; set; }
        public required string CreatedAt { get; set; }

        public required string Title { get; set; }
        public required string Body { get; set; }

        BlogDTOReturn() { }

        public static BlogDTOReturn FromBlog(Blog b) {
            return new BlogDTOReturn { Body = b.Body, Title = b.Title, UserId = b.Userid ?? "", CreatedAt = b.Createdat.ToString() };
        }
    }
};
