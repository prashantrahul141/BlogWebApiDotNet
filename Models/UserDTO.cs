namespace BlogWebApiDotNet.Models
{
    public class UserDTO
    {
        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public string Image { get; set; } = string.Empty;
    }

    public class UserPublicDTO
    {
        public required string UserId { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public string Avatar { get; set; } = string.Empty;
    }

    public class UserLeastImportantDTO
    {
        public required string Username { get; set; }
        public required string Avatar { get; set; } = string.Empty;
        public required string UserId { get; set; }

        public static UserLeastImportantDTO FromUser(AppUser user)
        {
            return new UserLeastImportantDTO()
            {
                Avatar = user.Image,
                Username = user.UserName ?? "",
                UserId = user.Id
            };
        }
    }
};
