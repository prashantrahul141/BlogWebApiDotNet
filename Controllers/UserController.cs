using BlogWebApiDotNet.Managers;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApiDotNet.Controllers
{
    /// <summar>
    /// class <c>UserController</c>, api endpoints for handling User data.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserManager m_userManager, ILogger<UserController> m_logger)
        : ControllerBase
    {
        private readonly AppUserManager userManager = (AppUserManager)m_userManager;

        // The dotnet core handles logging using this ILogger.
        private readonly ILogger<UserController> logger = m_logger;

        /// <summary>
        /// Gets currently logged in user info.
        /// </summary>
        [HttpGet("GetLoggedInUser"), Authorize]
        public async Task<ActionResult<UserDTO>> GetLoggedInUser()
        {
            return await userManager.GetLoggedInUser(User);
        }

        /// <summary>
        /// Gets currently logged in user's image.
        /// </summary>
        [HttpGet("GetLoggedInUserImage"), Authorize]
        public async Task<ActionResult<string>> GetLoggedInUserImage()
        {
            return await userManager.GetLoggedInUserImage(User);
        }

        /// <summary>
        // Update user in-place.
        /// </summary>
        [HttpPatch("UpdateUser"), Authorize]
        public async Task<ActionResult<UserDTO>> UpdateUser([FromBody] UserPublicDTO userNewData)
        {
            return await userManager.UpdateUser(User, userNewData);
        }

        /// <summary>
        /// Get user info from its id.
        /// </summary>
        [HttpGet("GetUserById/{userId}")]
        public async Task<ActionResult<UserPublicDTO>> GetUserById([FromRoute] string userId)
        {
            return await userManager.GetUserById(userId);
        }

        /// <summary>
        /// Get user info from its username.
        /// </summary>
        [HttpGet("GetUserByUsername/{username}")]
        public async Task<ActionResult<UserPublicDTO>> GetUserByUsername(
            [FromRoute] string username
        )
        {
            return await userManager.GetUserByUsername(username);
        }
    }
}
