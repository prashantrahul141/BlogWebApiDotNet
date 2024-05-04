using System.Security.Claims;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlogWebApiDotNet.Managers {

    public interface IUserManager {
        public Task<ActionResult<UserPublicDTO>> GetLoggedInUser(ClaimsPrincipal user);

        public Task<ActionResult<string>> GetLoggedInUserImage(ClaimsPrincipal user);

        public Task<ActionResult<string>> GetUserImage(string userId);

    }


    public class AppUserManager(DataContext m_dataContext, IUserStore<AppUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<AppUser> passwordHasher, IEnumerable<IUserValidator<AppUser>> userValidators, IEnumerable<IPasswordValidator<AppUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AppUser>> logger) : UserManager<AppUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger), IUserManager {
        private readonly DataContext _DbContext = m_dataContext;


        public async Task<ActionResult<UserPublicDTO>> GetLoggedInUser(ClaimsPrincipal user) {
            var userImage = await GetLoggedInUserImage(user);

            return new UserPublicDTO() {
                Email = GetUserClaimIdentity(user, ClaimTypes.Email),
                userId = GetUserClaimIdentity(user, ClaimTypes.NameIdentifier),
                Name = GetUserClaimIdentity(user, ClaimTypes.Name),
                Image = userImage.Value!
            };
        }

        public async Task<ActionResult<string>> GetLoggedInUserImage(ClaimsPrincipal user) {
            return await GetUserImage(GetUserClaimIdentity(user, ClaimTypes.NameIdentifier));
        }


        public async Task<ActionResult<string>> GetUserImage(string userId) {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) {
                return "https://avatars.githubusercontent.com/u/59825803?v=4";
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