namespace BlogWebApiDotNet.Models {
    public class BlogDTO {
        public required string Title { get; set; }
        public required string Body { get; set; }

    }

    public class BlogDTOReturn {
        public required long Id { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string CreatedAt { get; set; }
        public required string UserImage { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }


        BlogDTOReturn() { }

        public static BlogDTOReturn FromBlog(Blog b) {
            var Username = "unknown";
            var Image = "";
            if (b.User != null) {
                Username = b.User.UserName;
                Image = b.User.Image;
            }

#pragma warning disable CS8601 //  coulnt be null because we just checked the existence of user
            return new BlogDTOReturn { Id = b.Id, Body = b.Body, Title = b.Title, UserId = b.Userid ?? "", UserName = Username, CreatedAt = b.Createdat.ToString(), UserImage = Image };
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }
};
