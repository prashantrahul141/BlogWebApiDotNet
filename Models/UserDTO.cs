namespace BlogWebApiDotNet.Models {
    public class UserDTO {
        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public string Image { get; set; } = string.Empty;
    }

    public class UserPublicDTO {
        public required string userId { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public string Image { get; set; } = string.Empty;
    }
};
