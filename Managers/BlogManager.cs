using System.Security.Claims;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApiDotNet.Managers
{
    /// <summary>
    /// Interface <c>IBlogManager</c> provides all the public apis to perform actions on blogs.
    /// </summary>
    public interface IBlogManager
    {
        /// <summary>
        /// Method <c>GetAll</c> Retrives all the blogs present in the db.
        /// </summary>
        public Task<List<BlogDTOReturn>> GetAll();

        /// <summary>
        /// Method <c>GetByBlogId</c> Retrives blog of a specific id.
        /// </summary>
        /// <param name="BlogId">Id of the blog</param>
        public Task<ActionResult<BlogDTOReturn>> GetByBlogId(long BlogId);

        /// <summary>
        /// Method <c>GetByBlogId</c> Retrives all blogs by a specific user.
        /// </summary>
        /// <param name="UserId">Id of the user</param>
        public Task<ActionResult<List<BlogDTOReturn>>> GetByUserId(string UserId);

        /// <summary>
        /// Method <c>CreateNew</c> Creates a new blog.
        /// </summary>
        /// <param name="blogBody">blog data to create.</param>
        /// <param name="loggedInUserId">id of the logged in user.</param>
        public Task<ActionResult> CreateNew(BlogDTO blogBody, string loggedInUserId);

        /// <summary>
        /// Method <c>UpdateExisting</c> updates a blog in-place.
        /// </summary>
        /// <param name="blogId">blog id of the blog to update.</param>
        /// <param name="blogBody">BlogDto data to update..</param>
        /// <param name="currentUser">Info about the current logged in user.</param>
        /// <param name="loggedInUserId">id of the logged in user.</param>
        public Task<ActionResult> UpdateExisting(
            long blogId,
            BlogDTO blogBody,
            ClaimsPrincipal currentUser,
            string loggedInUserId
        );
    }

    /// <summary>
    /// Class <c>BlogManager</c> implements <c>IBlogManager</c>
    /// </summary>
    public class BlogManager : ControllerBase, IBlogManager
    {
        private readonly DataContext _DBContext;

        public BlogManager(DataContext m_dbContext)
        {
            _DBContext = m_dbContext;
            _DBContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Method <c>GetAll</c> Retrives all the blogs present in the db.
        /// </summary>
        public async Task<List<BlogDTOReturn>> GetAll()
        {
            return await _DBContext
                .Blogs.Include(e => e.User)
                .OrderByDescending(e => e.Createdat)
                .Select(e => BlogDTOReturn.FromBlog(e))
                .ToListAsync();
        }

        /// <summary>
        /// Method <c>GetByBlogId</c> Retrives blog of a specific id.
        /// </summary>
        /// <param name="BlogId">Id of the blog</param>
        public async Task<ActionResult<BlogDTOReturn>> GetByBlogId(long BlogId)
        {
            var result = await _DBContext
                .Blogs.Where(e => e.Id == BlogId)
                .Include(e => e.User)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return BlogDTOReturn.FromBlog(result);
        }

        /// <summary>
        /// Method <c>GetByBlogId</c> Retrives all blogs by a specific user.
        /// </summary>
        /// <param name="UserId">Id of the user</param>
        public async Task<ActionResult<List<BlogDTOReturn>>> GetByUserId(string UserId)
        {
            return await _DBContext
                .Blogs.Where(e => e.Userid == UserId)
                .Include(e => e.User)
                .Select(blog => BlogDTOReturn.FromBlog(blog))
                .ToListAsync();
        }

        /// <summary>
        /// Method <c>CreateNew</c> Creates a new blog.
        /// </summary>
        /// <param name="blogBody">blog data to create.</param>
        /// <param name="loggedInUserId">id of the logged in user.</param>
        public async Task<ActionResult> CreateNew(BlogDTO blogBody, string loggedInUserId)
        {
            var newBlog = Blog.FromModel(blogBody);

            var author = await _DBContext.Users.FirstAsync(x => x.Id == loggedInUserId);
            if (author == null)
            {
                return Forbid();
            }

            newBlog.User = author;

            try
            {
                await _DBContext.Blogs.AddAsync(newBlog);
                await _DBContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created, BlogDTOReturn.FromBlog(newBlog));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Method <c>UpdateExisting</c> updates a blog in-place.
        /// </summary>
        /// <param name="blogId">blog id of the blog to update.</param>
        /// <param name="blogBody">BlogDto data to update..</param>
        /// <param name="currentUser">Info about the current logged in user.</param>
        /// <param name="loggedInUserId">id of the logged in user.</param>
        public async Task<ActionResult> UpdateExisting(
            long blogId,
            BlogDTO blogBody,
            ClaimsPrincipal currentUser,
            string loggedInUserId
        )
        {
            var foundBlog = await _DBContext.Blogs.FirstAsync(e => e.Id == blogId);

            // checking if the blog exists.
            if (foundBlog == null)
            {
                return NotFound();
            }

            if (loggedInUserId != foundBlog.Userid)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            foundBlog.Title = blogBody.Title;
            foundBlog.Body = blogBody.Body;

            try
            {
                await _DBContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status202Accepted, foundBlog);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
