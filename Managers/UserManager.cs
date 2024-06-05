using System.Security.Claims;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApiDotNet.Managers
{
    /// <summary>
    /// Interface <c>IUserManager</c> provides all the public apis to perform actions on User.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Method <c>GetLoggedInUser</c> Retrives user info about the currenttly logged in user.
        /// </summary>
        /// <param name="user">ClaimsPrincipal object of the logged in user.</param>
        public Task<ActionResult<UserDTO>> GetLoggedInUser(ClaimsPrincipal user);

        /// <summary>
        /// Method <c>GetLoggedInUserImage</c> Retrives user image of the currenttly logged in user.
        /// </summary>
        /// <param name="user">ClaimsPrincipal object of the logged in user.</param>
        public Task<ActionResult<string>> GetLoggedInUserImage(ClaimsPrincipal user);

        /// <summary>
        /// Method <c>GetUserImage</c> Retrives user image of a user
        /// </summary>
        /// <param name="userId">User id</param>
        public Task<ActionResult<string>> GetUserImage(string userId);

        /// <summary>
        /// Method <c>UpdateUser</c> updates user In-place
        /// </summary>
        /// <param name="user">ClaimsPrincipal object of the user.</param>
        /// <param name="userNewData">data to update.</param>
        public Task<ActionResult<UserDTO>> UpdateUser(
            ClaimsPrincipal user,
            UserPublicDTO userNewData
        );

        /// <summary>
        /// Method <c>GetUserById</c> Gets user info.
        /// </summary>
        /// <param name="userId">User id to retrive.</param>
        public Task<ActionResult<UserPublicDTO>> GetUserById(string userId);

        /// <summary>
        /// Method <c>GetuserByUsername</c> Gets user info from username instead.
        /// </summary>
        /// <param name="username">Usernameto retrive.</param>
        public Task<ActionResult<UserPublicDTO>> GetUserByUsername(string username);
    }

    /// <summary>
    /// Class <c>AppUserManager</c> implements IUserManager
    /// </summary>
    public class AppUserManager(DataContext m_dataContext) : ControllerBase, IUserManager
    {
        private readonly DataContext _DbContext = m_dataContext;

        /// <summary>
        /// Method <c>UpdateUser</c> updates user In-place
        /// </summary>
        /// <param name="user">ClaimsPrincipal object of the user.</param>
        /// <param name="userNewData">data to update.</param>
        public async Task<ActionResult<UserDTO>> UpdateUser(
            ClaimsPrincipal user,
            UserPublicDTO userNewData
        )
        {
            var loggedInUser = await GetLoggedInUser(user);
            if (loggedInUser.Value == null)
            {
                return Unauthorized();
            }

            var loggedInUserObject = await _DbContext.Users.FirstOrDefaultAsync(user =>
                user.Id == loggedInUser.Value.UserId
            );
            if (loggedInUserObject == null)
            {
                return Unauthorized();
            }

            var existsUser = await _DbContext.Users.FirstOrDefaultAsync(user =>
                user.UserName == userNewData.Username && user.Id != loggedInUserObject.Id
            );
            if (existsUser != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Username already taken.");
            }

            loggedInUserObject.UserName = userNewData.Username;
            loggedInUserObject.NormalizedUserName = userNewData.Username.ToUpper();
            loggedInUserObject.Image = userNewData.Avatar;

            try
            {
                await _DbContext.SaveChangesAsync();
                return StatusCode(
                    StatusCodes.Status202Accepted,
                    new UserDTO()
                    {
                        UserId = loggedInUserObject.Id,
                        Email = loggedInUserObject.Email ?? "",
                        Username = loggedInUserObject.UserName ?? "",
                        Avatar = loggedInUserObject.Image
                    }
                );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Internal server error, failed to update uesr"
                );
            }
        }

        /// <summary>
        /// Method <c>GetLoggedInUser</c> Retrives user info about the currenttly logged in user.
        /// </summary>
        /// <param name="user">ClaimsPrincipal object of the logged in user.</param>
        public async Task<ActionResult<UserDTO>> GetLoggedInUser(ClaimsPrincipal user)
        {
            var claimUserId = GetUserClaimIdentity(user, ClaimTypes.NameIdentifier);
            var loggedInUser = await _DbContext.Users.FirstOrDefaultAsync(user =>
                user.Id == claimUserId
            );

            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            return new UserDTO()
            {
                UserId = loggedInUser.Id,
                Email = loggedInUser.Email ?? "",
                Username = loggedInUser.UserName ?? "",
                Avatar = loggedInUser.Image ?? "",
            };
        }

        /// <summary>
        /// Method <c>GetLoggedInUserImage</c> Retrives user image of the currenttly logged in user.
        /// </summary>
        /// <param name="user">ClaimsPrincipal object of the logged in user.</param>
        public async Task<ActionResult<string>> GetLoggedInUserImage(ClaimsPrincipal user)
        {
            return await GetUserImage(GetUserClaimIdentity(user, ClaimTypes.NameIdentifier));
        }

        /// <summary>
        /// Method <c>GetUserImage</c> Retrives user image of a user
        /// </summary>
        /// <param name="userId">User id</param>
        public async Task<ActionResult<string>> GetUserImage(string userId)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return "https://avatars.githubusercontent.com/u/59825803?v=4";
            }

            return user.Image;
        }

        /// <summary>
        /// Method <c>GetUserById</c> Gets user info.
        /// </summary>
        /// <param name="userId">User id to retrive.</param>
        public async Task<ActionResult<UserPublicDTO>> GetUserById(string userId)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            return UserPublicDTO.FromUser(user);
        }

        /// <summary>
        /// Method <c>GetuserByUsername</c> Gets user info from username instead.
        /// </summary>
        /// <param name="username">Usernameto retrive.</param>
        public async Task<ActionResult<UserPublicDTO>> GetUserByUsername(string username)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }

            return UserPublicDTO.FromUser(user);
        }

        private static string GetUserClaimIdentity(ClaimsPrincipal user, string claimType)
        {
            var claimsIdentityRaw =
                user.Identity ?? throw new Exception("Identity of user is null");
            var claimsIdentity =
                (ClaimsIdentity)claimsIdentityRaw
                ?? throw new Exception("Cannot convert Claims Identity.");
            var claims =
                claimsIdentity.FindFirst(claimType)
                ?? throw new Exception($"Cannot find name {claimType} of identity.");
            return claims.ToString().Split(":").Last().Trim();
        }
    }
}
