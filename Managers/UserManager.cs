using System.Security.Claims;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApiDotNet.Managers {

    public interface IUserManager {
        public ActionResult<UserPublicDTO> GetLoggedInUser(ClaimsPrincipal user);

        public Task<ActionResult<string>> GetLoggedInUserImage(ClaimsPrincipal user);
    }


    public class UserManager : ControllerBase, IUserManager {
        private readonly DataContext _DbContext;

        public UserManager(DataContext _dbcontext) {
            _DbContext = _dbcontext;
            _DbContext.Database.EnsureCreated();
        }

        public ActionResult<UserPublicDTO> GetLoggedInUser(ClaimsPrincipal user) {
            return new UserPublicDTO() {
                Email = GetUserClaimIdentity(user, ClaimTypes.Email),
                userId = GetUserClaimIdentity(user, ClaimTypes.NameIdentifier),
                Name = GetUserClaimIdentity(user, ClaimTypes.Name)
            };
        }

        public async Task<ActionResult<string>> GetLoggedInUserImage(ClaimsPrincipal user) {
            return await GetUserImage(GetUserClaimIdentity(user, ClaimTypes.NameIdentifier));
        }


        public async Task<ActionResult<string>> GetUserImage(string userId) {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) {
                return NotFound();
            }

            return user.Image;
        }


        private static string GetUserClaimIdentity(ClaimsPrincipal user, string claimType) {
            var claimsIdentityRaw = user.Identity ?? throw new Exception("Identity of user is null");
            var claimsIdentity = (ClaimsIdentity)claimsIdentityRaw ?? throw new Exception("Cannot convert Claims Identity.");
            var claims = claimsIdentity.FindFirst(claimType) ?? throw new Exception($"Cannot find name {claimType} of identity.");
            return claims.ToString().Split(":").Last().Trim();
        }

    }

}