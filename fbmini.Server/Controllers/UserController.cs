using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace fbmini.Server.Controllers
{
    public class UserEditView()
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public IFormFile? Picture { get; set; }
        public IFormFile? Cover { get; set; }
    }

    public class UserView()
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
    }

    public class PostView()
    {
        public required string Title { get; set; }
        public string? Content { get; set; }
        public IFormFile? Attachment { get; set; }
        public int? ParentPost { get; set; }
    }

    //public class PostShowView()
    //{
    //    public required int Id { get; set; }
    //    public required string Title { get; set; }
    //    public string? Content { get; set; }
    //    public required DateTime Date { get; set; }
    //    public IFormFile? Attachment { get; set; }
    //    public int? ParentPost { get; set; }
    //    public required UserShowView Poster { get; set; }
    //    public class UserShowView
    //    {
    //        public required string UserName { get; set; }
    //        public IFormFile? Picture { get; set; }
    //    }
    //    public int Likes { get; set; }
    //    public int Dislikes { get; set; }
    //    public int? Vote { get; set; }
    //    public required List<int> SubPostsIds { get; set; }
    //}

    [ApiController]
    [Route("api/[controller]")]
    public class UserController(UserManager<User> userManager, fbminiServerContext context) : ControllerBase
    {
        private string? GetUserID()
        {
            return User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<UserView>> Get()
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var user = await context.Users.Include(i => i.UserData).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
                return NotFound();

            var view = new UserView
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Bio = user.UserData.Bio,
            };

            return Ok(view);
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetUsernames()
        {
            var users = await context.Users
            .Select(u => u.UserName)
            .ToListAsync();

            return Ok(users);
        }

        [Authorize]
        [HttpGet("picture")]
        public async Task<IActionResult> GetPicture()
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await context.Users.Include(i => i.UserData.Picture).FirstOrDefaultAsync(user => user.Id == userId);

            if (user != null && user.UserData.Picture != null)
            {
                var stream = new MemoryStream(user.UserData.Picture.FileData);
                return File(stream.ToArray(), user.UserData.Picture.ContentType);
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet("cover")]
        public async Task<IActionResult> GetCover()
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await context.Users.Include(i => i.UserData.Cover).FirstOrDefaultAsync(user => user.Id == userId);

            if (user != null && user.UserData.Cover != null)
            {
                var stream = new MemoryStream(user.UserData.Cover.FileData);
                return File(stream.ToArray(), user.UserData.Cover.ContentType);
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<UserView>> GetUser(string username)
        {
            var user = await context.Users.Include(i => i.UserData).FirstOrDefaultAsync(user => user.UserName == username);

            if (user == null)
                return BadRequest();

            var view = new UserView
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Bio = user.UserData.Bio,
            };

            return Ok(view);
        }

        [Authorize]
        [HttpGet("{username}/picture")]
        public async Task<IActionResult> GetPicture(string username)
        {
            var user = await context.Users.Include(i => i.UserData.Picture).FirstOrDefaultAsync(user => user.UserName == username);

            if (user == null)
                return BadRequest();

            if (user.UserData.Picture != null)
            {
                var stream = new MemoryStream(user.UserData.Picture.FileData);
                return File(stream.ToArray(), user.UserData.Picture.ContentType);
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet("{username}/cover")]
        public async Task<IActionResult> GetCover(string username)
        {
            var user = await context.Users.Include(i => i.UserData.Cover).FirstOrDefaultAsync(user => user.UserName == username);

            if (user == null)
                return BadRequest();

            if (user.UserData.Cover != null)
            {
                var stream = new MemoryStream(user.UserData.Cover.FileData);
                return File(stream.ToArray(), user.UserData.Cover.ContentType);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserEditView userView)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return Unauthorized();

            if (userView.PhoneNumber != null)
            {
                var result = await userManager.SetPhoneNumberAsync(user, userView.PhoneNumber);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }

            if (userView.Email != null)
            {
                var result = await userManager.SetEmailAsync(user, userView.Email);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }

            // update user object to reflect the previous changes
            user = await context.Users.Include(user => user.UserData).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
                return Unauthorized();

            if (userView.Bio != null)
                user.UserData.Bio = userView.Bio;

            if (userView.Picture != null)
            {
                using var stream = new MemoryStream();
                await userView.Picture.CopyToAsync(stream);
                if (user.UserData.Picture == null)
                    user.UserData.Picture = new FileModel
                    {
                        FileName = userView.Picture.FileName,
                        ContentType = userView.Picture.ContentType,
                        Size = userView.Picture.Length,
                        FileData = stream.ToArray()
                    };
                else
                {
                    user.UserData.Picture.FileName = userView.Picture.FileName;
                    user.UserData.Picture.ContentType = userView.Picture.ContentType;
                    user.UserData.Picture.Size = userView.Picture.Length;
                    user.UserData.Picture.FileData = stream.ToArray();
                }
            }
            
            if (userView.Cover != null)
            {
                using var stream = new MemoryStream();
                await userView.Cover.CopyToAsync(stream);
                if (user.UserData.Cover == null)
                    user.UserData.Cover = new FileModel
                    {
                        FileName = userView.Cover.FileName,
                        ContentType = userView.Cover.ContentType,
                        Size = userView.Cover.Length,
                        FileData = stream.ToArray()
                    };
                else
                {
                    user.UserData.Cover.FileName = userView.Cover.FileName;
                    user.UserData.Cover.ContentType = userView.Cover.ContentType;
                    user.UserData.Cover.Size = userView.Cover.Length;
                    user.UserData.Cover.FileData = stream.ToArray();
                }
            }

            context.Users.Update(user);

            context.SaveChanges();

            return Ok(new { message = "Profile updated" });
        }

        [Authorize]
        [HttpPost("post")]
        public async Task<IActionResult> Post([FromForm] PostView postView)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await context.Users
                .Include(u => u.UserData)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            var post = new PostModel
            {
                PosterId = user.Id,
                Title = postView.Title,
                Content = postView.Content
            };

            if (postView.Attachment != null)
            {
                using var stream = new MemoryStream();
                await postView.Attachment.CopyToAsync(stream);

                post.Attachment = new FileModel
                {
                    FileName = postView.Attachment.FileName,
                    ContentType = postView.Attachment.ContentType,
                    Size = postView.Attachment.Length,
                    FileData = stream.ToArray()
                };
            }

            if (postView.ParentPost != null)
            {
                var parentPost = await context.Posts
                    .Include(p => p.SubPosts)
                    .FirstOrDefaultAsync(p => p.Id == postView.ParentPost);

                if (parentPost == null)
                    return NotFound(new { Message = "Parent post not found" });

                post.ParentPost = parentPost;
                parentPost.SubPosts.Add(post);

                context.Posts.Update(parentPost);
            }

            await context.Posts.AddAsync(post);
            await context.SaveChangesAsync();

            user.UserData.Posts.Add(post);
            context.UserData.Update(user.UserData);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Post created successfully", PostId = post.Id });
        }

        private object MapPostDetails(PostModel post)
        {
            var userId = GetUserID();

            // TODO
            //new PostShowView {
            //    Id = post.Id,
            //    Title = post.Title,
            //    Content = post.Content,
            //    Date = post.Date,
            //    Poster =
            //    new PostShowView.UserShowView {
            //       UserName = post.Poster.UserName,
            //       Picture = File(post.Poster.UserData.Picture.FileData, post.Poster.UserData.Picture.ContentType)
            //    },

            //}

            return new
            {
                post.Id,
                post.Title,
                post.Content,
                post.Date,
                Poster = new
                {
                    post.Poster.UserName,
                    Picture = post.Poster.UserData.Picture != null ? new
                    {
                        post.Poster.UserData.Picture.ContentType,
                        post.Poster.UserData.Picture.FileData,
                    } : null
                },
                Attachment = post.Attachment != null ? new
                {
                    post.Attachment.FileName,
                    post.Attachment.ContentType,
                    post.Attachment.Size,
                    post.Attachment.FileData,
                } : null,
                Likes = post.Likers.Count,
                Dislikes = post.Dislikers.Count,
                Vote = post.Likers.Any(e => e.Id == userId) ? 1 : post.Dislikers.Any(e => e.Id == userId) ? 0 : (int?)null,
                SubPostsIds = post.SubPosts.Select(p => p.Id).ToList(),
            };
        }

        private async Task<object?> GetPostFromId(int postId)
        {
            var post = await context.Posts
                        .Include(p => p.Poster)
                            .ThenInclude(u => u.UserData)
                                .ThenInclude(ud => ud.Cover)
                        .Include(p => p.Poster.UserData.Picture)
                        .Include(p => p.Attachment)
                        .Include(p => p.Likers)
                        .Include(p => p.Dislikers)
                        .Include(p => p.SubPosts)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return null;

            return MapPostDetails(post);
        }

        [Authorize]
        [HttpGet("post")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await context.Posts
            .Include(p => p.Poster)
                .ThenInclude(u => u.UserData)
                    .ThenInclude(ud => ud.Cover)
            .Include(p => p.Poster.UserData.Picture)
            .Include(p => p.Attachment)
            .Include(p => p.Likers)
            .Include(p => p.Dislikers)
            .Include(p => p.SubPosts)
            .AsNoTracking()
            .Where(p => p.ParentPostId == null)
            .OrderByDescending(p => p.Date)
            .ToListAsync();

            return Ok(posts.Select(MapPostDetails).ToList());
        }

        [Authorize]
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetPost(int postId)
        {
            var result = await GetPostFromId(postId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("subpost/{postId}")]
        public async Task<IActionResult> GetComments(int postId)
        {
            var post = await context.Posts
                .Include(p=>p.SubPosts)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return NotFound();
            
            var results = new List<object?>();

            foreach (var p in post.SubPosts)
                results.Add(await GetPostFromId(p.Id));

            return Ok(results);
        }

        [Authorize]
        [HttpPost("vote/{value}/{postId}")]
        public async Task<IActionResult> PostVote(int value, int postId)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await context.Users
                .Include(u => u.LikedPosts)
                .Include(u => u.DislikedPosts)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            var post = await context.Posts
                .Include(p => p.Likers)
                .Include(p => p.Dislikers)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return NotFound();

            switch(value)
            {
                case 0:
                    if (!user.DislikedPosts.Remove(post))
                    {
                        user.LikedPosts.Remove(post);
                        user.DislikedPosts.Add(post);
                    }
                    break;

                case 1:
                    if (!user.LikedPosts.Remove(post))
                    {
                        user.DislikedPosts.Remove(post);
                        user.LikedPosts.Add(post);
                    }
                    break;
                default:
                    return BadRequest();
            }

            context.Users.Update(user);
            context.Posts.Update(post);
            await context.SaveChangesAsync();

            return Ok(new { Likes = post.Likers.Count, Dislikes = post.Dislikers.Count, Vote = 0 });
        }

        [Authorize]
        [HttpGet("vote/{postId}")]
        public async Task<IActionResult> GetVote(int postId)
        {
            var post = await context.Posts
                .Include(p => p.Likers)
                .Include(p => p.Dislikers)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return NotFound();

            return Ok(new { Likes = post.Likers.Count, Dislikes = post.Dislikers.Count });
        }



        [HttpGet("name")]
        public async Task<IActionResult> GetUsername()
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await context.Users.Select(u => new { u.Id, u.UserName })
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            return Ok(new { user.UserName });
        }
    }
}
