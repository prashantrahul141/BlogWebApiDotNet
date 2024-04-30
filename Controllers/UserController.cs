using BlogWebApiDotNet.Managers;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApiDotNet.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserManager m_userManager, ILogger<UserController> m_logger) : ControllerBase {
        private readonly UserManager userManager = (UserManager)m_userManager;

        // The dotnet core handles logging using this ILogger.
        private readonly ILogger<UserController> logger = m_logger;



        [HttpGet("GetLoggedInUser"), Authorize]
        public ActionResult<UserPublicDTO> GetLoggedInUser() {
            return userManager.GetLoggedInUser(User);
        }


        [HttpGet("GetLoggedInUserImage"), Authorize]
        public async Task<ActionResult<string>> GetLoggedInUserImage() {
            return await userManager.GetLoggedInUserImage(User);
        }

    }

}