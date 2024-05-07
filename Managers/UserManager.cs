using System.Security.Claims;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApiDotNet.Managers
{
    public interface IUserManager
    {
        public Task<ActionResult<UserPublicDTO>> GetLoggedInUser(ClaimsPrincipal user);

        public Task<ActionResult<string>> GetLoggedInUserImage(ClaimsPrincipal user);

        public Task<ActionResult<string>> GetUserImage(string userId);

        public Task<ActionResult<UserPublicDTO>> UpdateUser(
            ClaimsPrincipal user,
            UserLeastImportantDTO userNewData
        );

        public Task<ActionResult<UserLeastImportantDTO>> GetUserById(string userId);
    }

    public class AppUserManager(DataContext m_dataContext) : ControllerBase, IUserManager
    {
        private readonly DataContext _DbContext = m_dataContext;

        public async Task<ActionResult<UserPublicDTO>> UpdateUser(
            ClaimsPrincipal user,
            UserLeastImportantDTO userNewData
        )
        {
            var loggedInUser = await GetLoggedInUser(user);
            if (loggedInUser.Value == null)
            {
                return Unauthorized();
            }

            var loggedInUserObject = await _DbContext.Users.FirstOrDefaultAsync(user =>
                user.Id == loggedInUser.Value.userId
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
                    new UserPublicDTO()
                    {
                        userId = loggedInUserObject.Id,
                        Email = loggedInUserObject.Email ?? "",
                        Name = loggedInUserObject.UserName ?? "",
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

        public async Task<ActionResult<UserPublicDTO>> GetLoggedInUser(ClaimsPrincipal user)
        {
            var claimUserId = GetUserClaimIdentity(user, ClaimTypes.NameIdentifier);
            var loggedInUser = await _DbContext.Users.FirstOrDefaultAsync(user =>
                user.Id == claimUserId
            );

            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            return new UserPublicDTO()
            {
                userId = loggedInUser.Id,
                Email = loggedInUser.Email ?? "",
                Name = loggedInUser.UserName ?? "",
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

        public async Task<ActionResult<UserLeastImportantDTO>> GetUserById(string userId)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            return UserLeastImportantDTO.FromUser(user);
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
