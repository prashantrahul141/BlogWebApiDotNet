namespace BlogWebApiDotNet.Models
{
    /// <summary>
    /// Class <c>RegisterModel</c> Models registration form data.
    /// </summary>
    public class RegisterModel
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
