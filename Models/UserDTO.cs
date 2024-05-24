namespace BlogWebApiDotNet.Models
{
    /// <summary>
    /// Class <c>UserDTO</c> is a DTO for User object, but only for the logged in user,
    /// since it contains the email value which should not be public.
    /// </summary>
    public class UserDTO
    {
        public required string UserId { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public string Avatar { get; set; } = string.Empty;
    }

    /// <summary>
    /// Class <c>UserPublicDTO</c> is a DTO for User object, for public usage,
    /// and can be shared publicly, does not contain any sensitive data.
    /// </summary>
    public class UserPublicDTO
    {
        public required string Username { get; set; }
        public required string Avatar { get; set; } = string.Empty;
        public required string UserId { get; set; }

        /// <summary>
        /// Static method <c>FromUser</c> converts AppUser object into UserPublicDTO
        ///  </summary>
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
