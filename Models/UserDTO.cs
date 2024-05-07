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
        public required string userId { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public string Image { get; set; } = string.Empty;
    }

    public class UserLeastImportantDTO
    {
        public required string Username { get; set; }

        public required string Avatar { get; set; } = string.Empty;

        public static UserLeastImportantDTO FromUser(AppUser user)
        {
            return new UserLeastImportantDTO()
            {
                Avatar = user.Image,
                Username = user.UserName ?? ""
            };
        }
    }
};
