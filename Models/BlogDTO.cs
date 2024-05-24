namespace BlogWebApiDotNet.Models
{
    /// <summary>
    /// Class <c>BlogDTO</c> Models creating new blog by logged in user.
    /// </summary>
    public class BlogDTO
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
    }

    /// <summary>
    /// Class <c>BlogDTOReturn</c>  maps 1-1 with the actual blog data.
    /// </summary>
    public class BlogDTOReturn
    {
        public required long Id { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string CreatedAt { get; set; }
        public required string UserImage { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }

        BlogDTOReturn() { }

        /// <summary>
        /// Static method <c>FromBlog</c> helper function which converts Blog object into BlogDTOReturn
        ///  </summary>
        public static BlogDTOReturn FromBlog(Blog b)
        {
            var Username = "unknown";
            var Image = "";
            if (b.User != null)
            {
                Username = b.User.UserName;
                Image = b.User.Image;
            }

#pragma warning disable CS8601 //  coulnt be null because we just checked the existence of user
            return new BlogDTOReturn
            {
                Id = b.Id,
                Body = b.Body,
                Title = b.Title,
                UserId = b.Userid ?? "",
                UserName = Username,
                CreatedAt = b.Createdat.ToString(),
                UserImage = Image
            };
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }
};
