using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace fbmini.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PostController(fbminiServerContext context) : HomeController
    {
        [HttpPost("Create")]
        public async Task<IActionResult> CreatePost([FromForm] PostForm postForm)
        {
            var userId = GetUserID();

            var user =
            (
                await context.Users
                    .Include(u => u.UserData)
                    .FirstOrDefaultAsync(u => u.Id == userId)
            )!;

            var post = await PostModel.FromFormAsync(postForm, userId);

            await context.Posts.AddAsync(post);
            await context.SaveChangesAsync();

            user.UserData.Posts.Add(post);
            context.UserData.Update(user.UserData);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Post created successfully", PostId = post.Id });
        }

        [HttpPost("Create/{parentId}")]
        public async Task<IActionResult> CreateComment([FromForm] PostForm postForm, int parentId)
        {
            var parentPost = await context.Posts
                    .Include(p => p.SubPosts)
                    .FirstOrDefaultAsync(p => p.Id == parentId);

            if (parentPost == null)
                return NotFound(new { Message = "Parent post not found" });

            var userId = GetUserID();

            var user =
            (
                await context.Users
                    .Include(u => u.UserData)
                    .FirstOrDefaultAsync(u => u.Id == userId)
            )!;

            var post = await PostModel.FromFormAsync(postForm, userId);

            post.ParentPost = parentPost;
            parentPost.SubPosts.Add(post);

            context.Posts.Update(parentPost);

            await context.Posts.AddAsync(post);
            await context.SaveChangesAsync();

            user.UserData.Posts.Add(post);
            context.UserData.Update(user.UserData);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Comment created successfully", PostId = post.Id });
        }

        [HttpGet("Vote/{postId}")]
        public async Task<IActionResult> GetVote(int postId)
        {
            var userId = GetUserID();

            var post = await context.Posts
                .Include(p => p.Likers)
                .Include(p => p.Dislikers)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return NotFound();

            return Ok(
            new {
                Likes = post.Likers.Count,
                Dislikes = post.Dislikers.Count,
                Vote = post.Likers.Any(e => e.Id == userId) ? 1 : post.Dislikers.Any(e => e.Id == userId) ? 0 : (int?)null
            });
        }

        [HttpPatch("Vote/{postId}/{value}")]
        public async Task<IActionResult> PostVote(int postId, int? value)
        {
            var userId = GetUserID();

            var user = (await context.Users
                .Include(u => u.LikedPosts)
                .Include(u => u.DislikedPosts)
                .FirstOrDefaultAsync(u => u.Id == userId))!;

            var post = await context.Posts
                .Include(p => p.Likers)
                .Include(p => p.Dislikers)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return NotFound();

            switch (value)
            {
                case 0:
                    if (!user.DislikedPosts.Remove(post))
                    {
                        user.LikedPosts.Remove(post);
                        user.DislikedPosts.Add(post);
                        value = 0;
                    }
                    else value = null;
                    break;

                case 1:
                    if (!user.LikedPosts.Remove(post))
                    {
                        user.DislikedPosts.Remove(post);
                        user.LikedPosts.Add(post);
                        value = 1;
                    }
                    else value = null;
                    break;
                default:
                    return BadRequest();
            }

            context.Users.Update(user);
            context.Posts.Update(post);
            await context.SaveChangesAsync();

            return Ok(
            new
            {
                Likes = post.Likers.Count,
                Dislikes = post.Dislikers.Count,
                Vote = value
            });
        }

        [HttpGet("Get/{postId}")]
        public async Task<ActionResult<PostContentResult>> GetPost(int postId)
        {
            var post = await context.Posts
            .AsNoTracking()
            .Include(p => p.Poster)
                .ThenInclude(u => u.UserData)
                    .ThenInclude(ud => ud.Cover)
            .Include(p => p.Poster.UserData.Picture)
            .Include(p => p.Attachment)
            .Include(p => p.Likers)
            .Include(p => p.Dislikers)
            .Include(p => p.SubPosts)
            .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return NotFound();

            var canEdit = GetUserID()! == post.PosterId || IsInRole("Manager") || IsInRole("Admin");

            return Ok(post.ToContentResult(canEdit));
        }


        [HttpGet("List/{postId}")]
        public async Task<ActionResult<List<PostContentResult>>> GetPosts(int? postId)
        {
            var userId = GetUserID();
            var isManagerOrAdmin = IsInRole("Manager") || IsInRole("Admin");

            var posts = await context.Posts
                .AsNoTracking()
                .Where(p => p.ParentPostId == postId)
                .OrderByDescending(p => p.Date)
                .Include(p => p.Poster)
                    .ThenInclude(u => u.UserData)
                    .ThenInclude(ud => ud.Cover)
                .Include(p => p.Poster.UserData.Picture)
                .Include(p => p.Attachment)
                .Include(p => p.Likers)
                .Include(p => p.Dislikers)
                .Include(p => p.SubPosts)
                .Select(p => p.ToContentResult(userId == p.PosterId || isManagerOrAdmin))
                .ToListAsync();

            return Ok(posts);
        }

        [HttpGet("List")]
        public async Task<ActionResult<List<PostContentResult>>> GetPosts()
        {
            return await GetPosts(null);
        }

        [HttpDelete("Delete/{postId}")]
        public async Task<ActionResult<PostContentResult>> DeletePost(int postId)
        {
            var post = await context.Posts
            .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return NotFound();

            if (!(GetUserID() == post.PosterId || IsInRole("Manager") || IsInRole("Admin")))
                return Unauthorized();

            context.Posts.Remove(post);

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
