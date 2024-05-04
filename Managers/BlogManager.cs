using System.Security.Claims;
using BlogWebApiDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApiDotNet.Managers {
    public interface IBlogManager {
        public Task<List<BlogDTOReturn>> GetAll();

        public Task<ActionResult<Blog>> GetByBlogId(long BlogId);

        public Task<ActionResult<List<Blog>>> GetByUserId(string UserId);

        public Task<ActionResult> CreateNew(BlogDTO blogBody, string loggedInUserId);

        public Task<ActionResult> UpdateExisting(long blogId, BlogDTO blogBody, ClaimsPrincipal currentUser, string loggedInUserId);

    }

    public class BlogManager : ControllerBase, IBlogManager {
        private readonly DataContext _DBContext;

        public BlogManager(DataContext m_dbContext) {
            _DBContext = m_dbContext;
            _DBContext.Database.EnsureCreated();
        }

        public async Task<List<BlogDTOReturn>> GetAll() {
            return await _DBContext.Blogs.Include(e => e.User).Select(e => BlogDTOReturn.FromBlog(e)).ToListAsync();
        }

        public async Task<ActionResult<Blog>> GetByBlogId(long BlogId) {
            var result = await _DBContext.Blogs.Where(e => e.Id == BlogId).FirstOrDefaultAsync();
            if (result == null) {
                return NotFound();
            }

            return result;
        }


        public async Task<ActionResult<List<Blog>>> GetByUserId(string UserId) {
            return await _DBContext.Blogs.Where(e => e.Userid == UserId).ToListAsync();
        }

        public async Task<ActionResult> CreateNew(BlogDTO blogBody, string loggedInUserId) {

            var newBlog = Blog.FromModel(blogBody);

            var author = await _DBContext.Users.FirstAsync(x => x.Id == loggedInUserId);
            if (author == null) {
                return Forbid();
            }

            newBlog.User = author;

            try {
                await _DBContext.Blogs.AddAsync(newBlog);
                await _DBContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created, BlogDTOReturn.FromBlog(newBlog));

            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ActionResult> UpdateExisting(long blogId, BlogDTO blogBody, ClaimsPrincipal currentUser, string loggedInUserId) {

            var foundBlog = await _DBContext.Blogs.FirstAsync(e => e.Id == blogId);

            // checking if the blog exists.
            if (foundBlog == null) {
                return NotFound();
            }

            if (loggedInUserId != foundBlog.Userid) {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            foundBlog.Title = blogBody.Title;
            foundBlog.Body = blogBody.Title;

            try {
                await _DBContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status202Accepted, foundBlog);

            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }

}