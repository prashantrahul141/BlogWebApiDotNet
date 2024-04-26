using System.Collections;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApiDotNet.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase {

        [HttpGet("GetLoggedInUser"), Authorize]
        public ActionResult GetLoggedInUser() {
            var userData = new Hashtable() {
                {"userId",GetUserClaimIdentity(ClaimTypes.NameIdentifier)},
                {"email",GetUserClaimIdentity(ClaimTypes.Email)},
                {"name",GetUserClaimIdentity(ClaimTypes.Name)}
            };

            return StatusCode(StatusCodes.Status200OK, JsonSerializer.Serialize(userData));

        }


        private string GetUserClaimIdentity(string claimType) {
            var claimsIdentityRaw = User.Identity ?? throw new Exception("Identity of user is null");
            var claimsIdentity = (ClaimsIdentity)claimsIdentityRaw ?? throw new Exception("Cannot convert Claims Identity.");
            var claims = claimsIdentity.FindFirst(claimType) ?? throw new Exception($"Cannot find name {claimType} of identity.");
            return claims.ToString().Split(":").Last().Trim();
        }



    }

}