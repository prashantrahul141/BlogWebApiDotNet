using BlogWebApiDotNet.Managers;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApiDotNet.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserManager m_userManager, ILogger<UserController> m_logger, SignInManager<AppUser> m_signInManager) : ControllerBase {
        private readonly AppUserManager customUserManager = (AppUserManager)m_userManager;
        private readonly SignInManager<AppUser> signInManager = m_signInManager;

        // The dotnet core handles logging using this ILogger.
        private readonly ILogger<UserController> logger = m_logger;

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterModel registerModel) {
            var newUser = new AppUser() {
                UserName = registerModel.UserName ?? "unknown",
                Email = registerModel.Email,
                PasswordHash = registerModel.Password
            };

            var result = await customUserManager.CreateAsync(newUser, newUser.PasswordHash!);

            if (result.Succeeded) {
                return Ok("Registed new user.");
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }



        [HttpGet("GetLoggedInUser"), Authorize]
        public async Task<ActionResult<UserPublicDTO>> GetLoggedInUser() {
            return await customUserManager.GetLoggedInUser(User);
        }


        [HttpGet("GetLoggedInUserImage"), Authorize]
        public async Task<ActionResult<string>> GetLoggedInUserImage() {
            return await customUserManager.GetLoggedInUserImage(User);
        }

    }

}