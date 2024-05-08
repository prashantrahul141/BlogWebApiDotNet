namespace BlogWebApiDotNet.Models
{
    public class UserDTO
    {
        public required string UserId { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public string Avatar { get; set; } = string.Empty;
    }

    public class UserPublicDTO
    {
        public required string Username { get; set; }
        public required string Avatar { get; set; } = string.Empty;
        public required string UserId { get; set; }

        public static UserPublicDTO FromUser(AppUser user)
        {
            return new UserPublicDTO()
            {
                Avatar = user.Image,
                Username = user.UserName ?? "",
                UserId = user.Id
            };
        }
    }
};
