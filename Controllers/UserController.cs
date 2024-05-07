using BlogWebApiDotNet.Managers;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApiDotNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserManager m_userManager, ILogger<UserController> m_logger)
        : ControllerBase
    {
        private readonly AppUserManager userManager = (AppUserManager)m_userManager;

        // The dotnet core handles logging using this ILogger.
        private readonly ILogger<UserController> logger = m_logger;

        [HttpGet("GetLoggedInUser"), Authorize]
        public async Task<ActionResult<UserPublicDTO>> GetLoggedInUser()
        {
            return await userManager.GetLoggedInUser(User);
        }

        [HttpGet("GetLoggedInUserImage"), Authorize]
        public async Task<ActionResult<string>> GetLoggedInUserImage()
        {
            return await userManager.GetLoggedInUserImage(User);
        }

        [HttpPatch("UpdateUser"), Authorize]
        public async Task<ActionResult<UserPublicDTO>> UpdateUser(
            [FromBody] UserLeastImportantDTO userNewData
        )
        {
            return await userManager.UpdateUser(User, userNewData);
        }

        [HttpGet("GetUserById/{userId}")]
        public async Task<ActionResult<UserLeastImportantDTO>> GetUserById(
            [FromRoute] string userId
        )
        {
            return await userManager.GetUserById(userId);
        }

        [HttpGet("GetUserByUsername/{username}")]
        public async Task<ActionResult<UserLeastImportantDTO>> GetUserByUsername(
            [FromRoute] string username
        )
        {
            return await userManager.GetUserByUsername(username);
        }
    }
}
