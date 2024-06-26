using System.Security.Claims;
using BlogWebApiDotNet.Managers;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApiDotNet.Controllers
{
    /// <summar>
    /// class <c>BlogsController</c>, api endpoints for handling User data.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController(IBlogManager m_blogManager, ILogger<BlogsController> m_logger)
        : ControllerBase
    {
        private readonly BlogManager blogManager = (BlogManager)m_blogManager;

        // The dotnet core handles logging using this ILogger.
        private readonly ILogger<BlogsController> logger = m_logger;


        /// <summary>
        /// Gets all blogs
        /// </summary>
        [HttpGet("GetAll")]
        public async Task<List<BlogDTOReturn>> GetAllBlogs()
        {
            return await blogManager.GetAll();
        }

        /// <summary>
        /// Gets a specific blogs using its id.
        /// </summary>
        [HttpGet("GetBlogByBlogId/{BlogId:long}")]
        public async Task<ActionResult<BlogDTOReturn>> GetBlogByBlogId([FromRoute] long BlogId)
        {
            return await blogManager.GetByBlogId(BlogId);
        }

        /// <summary>
        /// Gets all blogs by a specific user usings its id.
        /// </summary>
        [HttpGet("GetBlogByUserId/{UserId}")]
        public async Task<ActionResult<List<BlogDTOReturn>>> GetBlogByUserId(
            [FromRoute] string UserId
        )
        {
            return await blogManager.GetByUserId(UserId);
        }

        /// <summary>
        /// Create a new blog.
        /// </summary>
        [HttpPost("CreateNew"), Authorize]
        public async Task<ActionResult> CreateNew([FromBody] BlogDTO blogBody)
        {
            return await blogManager.CreateNew(blogBody, GetLoggedInUserId());
        }


        /// <summary>
        /// Updates a blog in-place.
        /// </summary>
        [HttpPatch("UpdateExisting/{blogId:long}"), Authorize]
        public async Task<ActionResult> UpdateExisting(
            [FromRoute] long blogId,
            [FromBody] BlogDTO blogBody
        )
        {
            return await blogManager.UpdateExisting(blogId, blogBody, User, GetLoggedInUserId());
        }

        private string GetLoggedInUserId()
        {
            var claimsIdentityRaw =
                User.Identity ?? throw new Exception("Identity of user is null");
            var claimsIdentity =
                (ClaimsIdentity)claimsIdentityRaw
                ?? throw new Exception("Cannot convert Claims Identity.");
            var claims =
                claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new Exception("Cannot find name identifier of identity.");
            return claims.ToString().Split(":").Last().Trim();
        }
    }
}
