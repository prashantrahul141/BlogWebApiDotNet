using System.Security.Claims;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApiDotNet.Managers
{
    public interface IUserManager
    {
        public Task<ActionResult<UserDTO>> GetLoggedInUser(ClaimsPrincipal user);

        public Task<ActionResult<string>> GetLoggedInUserImage(ClaimsPrincipal user);

        public Task<ActionResult<string>> GetUserImage(string userId);

        public Task<ActionResult<UserDTO>> UpdateUser(
            ClaimsPrincipal user,
            UserPublicDTO userNewData
        );

        public Task<ActionResult<UserPublicDTO>> GetUserById(string userId);

        public Task<ActionResult<UserPublicDTO>> GetUserByUsername(string username);
    }

    public class AppUserManager(DataContext m_dataContext) : ControllerBase, IUserManager
    {
        private readonly DataContext _DbContext = m_dataContext;

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
                        Image = loggedInUserObject.Image
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
                Image = loggedInUser.Image ?? "",
            };
        }

        public async Task<ActionResult<string>> GetLoggedInUserImage(ClaimsPrincipal user)
        {
            return await GetUserImage(GetUserClaimIdentity(user, ClaimTypes.NameIdentifier));
        }

        public async Task<ActionResult<string>> GetUserImage(string userId)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return "https://avatars.githubusercontent.com/u/59825803?v=4";
            }

            return user.Image;
        }

        public async Task<ActionResult<UserPublicDTO>> GetUserById(string userId)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            return UserPublicDTO.FromUser(user);
        }

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
