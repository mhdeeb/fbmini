using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System;


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

            var userId = GetUserID()!;

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
                        value = -1;
                    }
                    else value = 0;
                    break;

                case 1:
                    if (!user.LikedPosts.Remove(post))
                    {
                        user.DislikedPosts.Remove(post);
                        user.LikedPosts.Add(post);
                        value = 1;
                    }
                    else value = 0;
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

            var userId = GetUserID()!;

            var canEdit = userId == post.PosterId || IsInRole("Manager") || IsInRole("Admin");

            var result = post.ToContentResult(canEdit);

            result.Vote = post.Likers.Any(u => u.Id == userId) ? 1 : post.Dislikers.Any(u => u.Id == userId) ? -1 : 0; 

            return Ok(result);
        }

        [HttpGet("Parent/{postId}")]
        public async Task<ActionResult<int?>> GetParentPostId(int postId)
        {
            var post = await context.Posts
                .Where(p => p.Id == postId)
                .FirstAsync();

            if (post == null)
                return NotFound();

            return Ok(new { post.ParentPostId });
        }

        private async Task<List<PostContentResult>> GetPosts(string where)
        {
            List<PostContentResult> posts = [];
            var userId = GetUserID();
            var isManagerOrAdmin = IsInRole("Manager") || IsInRole("Admin");
            string query = $"""
            WITH BasePosts AS(
                SELECT
                    Id, Title, Content, [Date], PosterId, AttachmentId
                FROM
                    Posts
                WHERE
                    {where}
            ),
            LikesDislikes AS(
                SELECT
                    p.Id AS PostId,
                    COUNT(pl.LikersId) AS Likes,
                    COUNT(pd.DislikersId) AS Dislikes,
                    CASE
                        WHEN COUNT(CASE WHEN pl.LikersId = '{userId}' THEN 1 END) > 0 THEN 1
                        WHEN COUNT(CASE WHEN pd.DislikersId = '{userId}' THEN 1 END) > 0 THEN -1
                        ELSE 0
                    END AS PosterVote
                FROM
                    Posts p
                LEFT JOIN PostLikers AS pl ON p.Id = pl.LikedPostsId
                LEFT JOIN PostDislikers AS pd ON p.Id = pd.DislikedPostsId
                WHERE
                    {where}
                GROUP BY p.Id
            ),
            ChildPosts AS(
                SELECT
                    ParentPostId,
                    STRING_AGG(Id, ', ') AS ChildPostIds
                FROM
                    Posts
                WHERE
                    ParentPostId IS NOT NULL
                GROUP BY ParentPostId
            )
            SELECT
                p.Id            AS PostId,
                p.Title         AS PostTitle,
                p.Content       AS PostContent,
                p.[Date]        AS PostDate,
                p.AttachmentId  AS AttachmentId,
                a.UserName      AS PosterUserName,
                u.PictureId     AS PictureId,
                ld.Likes        AS Likes,
                ld.Dislikes     AS Dislikes,
                ld.PosterVote   AS Vote,
                cp.ChildPostIds AS ChildPostIds,
                a.Id            AS PosterId
            FROM
                BasePosts p
                INNER JOIN  AspNetUsers a     ON p.PosterId = a.Id
                LEFT JOIN   LikesDislikes ld  ON p.Id = ld.PostId
                LEFT JOIN   UserData u        ON a.Id = u.UserId
                LEFT JOIN   ChildPosts cp     ON cp.ParentPostId = p.Id
            ORDER BY
                p.[Date] DESC;
            """;

            using (var connection = context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = query;

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    posts.Add(new PostContentResult
                    {
                        Id = (reader.IsDBNull(0) ? null : reader.GetInt32(0)),
                        Title = reader.GetString(1),
                        Content = (reader.IsDBNull(2) ? null : reader.GetString(2)),
                        Date = (reader.IsDBNull(3) ? null : reader.GetDateTime(3)),
                        AttachmentUrl = (reader.IsDBNull(4) ? null : FileModel.GetUrl(reader.GetInt32(4))),
                        Poster = new UserContentResult
                        {
                            UserName = (reader.IsDBNull(5) ? null : reader.GetString(5)),
                            PictureUrl = (reader.IsDBNull(6) ? null : FileModel.GetUrl(reader.GetInt32(6)))
                        },
                        Likes = (reader.IsDBNull(7) ? null : reader.GetInt32(7)),
                        Dislikes = (reader.IsDBNull(8) ? null : reader.GetInt32(8)),
                        Vote = reader.GetInt32(9),
                        SubPostsIds = (reader.IsDBNull(10) ? [] : reader.GetString(10).Split(", ").Select(num => int.Parse(num.Trim())).ToList()),
                        CanEdit = userId == reader.GetString(11) || isManagerOrAdmin
                    });
                }
            }

            //new PostContentResult
            //{
            //    Id = bp.Id,
            //    Title = bp.Title,
            //    Content = bp.Content,
            //    Date = bp.Date,
            //    AttachmentUrl = attachmentFile != null ? attachmentFile.GetUrl() : null,
            //    Poster = new UserContentResult
            //    {
            //        UserName = user.UserName,
            //        PictureUrl = pictureFile != null ? pictureFile.GetUrl() : null
            //    },
            //    Likes = bp.Likers != null ? bp.Likers.Count() : 0,
            //    Dislikes = bp.Dislikers != null ? bp.Dislikers.Count() : 0,
            //    SubPostsIds = bp.SubPosts != null ? bp.SubPosts.ToList() : new List<int>(),
            //    CanEdit = userId == bp.PosterId || isManagerOrAdmin
            //}

            //var basePosts = context.Posts
            //    .Where(p => p.ParentPostId == null)
            //    .Select(p => new
            //    {
            //        p.Id,
            //        p.Title,
            //        p.Content,
            //        p.Date,
            //        p.PosterId,
            //        p.AttachmentId,
            //        p.Likers,
            //        p.Dislikers,
            //        SubPosts = p.SubPosts.Select(sp => sp.Id)
            //    });

            //var query = from bp in basePosts
            //            join user in context.Users on bp.PosterId equals user.Id
            //            join userData in context.UserData on user.Id equals userData.UserId into udJoin
            //            from userData in udJoin.DefaultIfEmpty()
            //            join pictureFile in context.Files on userData.PictureId equals pictureFile.Id into pfJoin
            //            from pictureFile in pfJoin.DefaultIfEmpty()
            //            join attachmentFile in context.Files on bp.AttachmentId equals attachmentFile.Id into afJoin
            //            from attachmentFile in afJoin.DefaultIfEmpty()
            //            orderby bp.Date descending
            //            select new PostContentResult
            //            {
            //                Id = bp.Id,
            //                Title = bp.Title,
            //                Content = bp.Content,
            //                Date = bp.Date,
            //                AttachmentUrl = attachmentFile != null ? attachmentFile.GetUrl() : null,
            //                Poster = new UserContentResult
            //                {
            //                    UserName = user.UserName,
            //                    PictureUrl = pictureFile != null ? pictureFile.GetUrl() : null
            //                },
            //                Likes = bp.Likers != null ? bp.Likers.Count() : 0,
            //                Dislikes = bp.Dislikers != null ? bp.Dislikers.Count() : 0,
            //                SubPostsIds = bp.SubPosts != null ? bp.SubPosts.ToList() : new List<int>(),
            //                CanEdit = userId == bp.PosterId || isManagerOrAdmin
            //            };

            //var posts = await query.ToListAsync();


            //var posts = await context.Posts
            //    .AsNoTracking()
            //    .Where(p => p.ParentPostId == postId)
            //    .OrderByDescending(p => p.Date)
            //    .Include(p => p.Poster)
            //        .ThenInclude(u => u.UserData)
            //        .ThenInclude(ud => ud.Cover)
            //    .Include(p => p.Poster.UserData.Picture)
            //    .Include(p => p.Attachment)
            //    .Include(p => p.Likers)
            //    .Include(p => p.Dislikers)
            //    .Include(p => p.SubPosts)
            //    .Select(p => p.ToContentResult(userId == p.PosterId || isManagerOrAdmin))
            //    .ToListAsync();

            return posts;
        }

        [HttpGet("User/{userName}")]
        public async Task<ActionResult<PostContentResult>> GetPostsByPoster(string? userName)
        {
            var userId = await context.Users
                .Where(u => u.UserName == userName)
                .Select(u => u.Id)
                .FirstAsync();
            return Ok(await GetPosts(userName == null ? $"PosterId = '{GetUserID()}'" : $"PosterId = '{userId}'"));
        }

        [HttpGet("User")]
        public async Task<ActionResult<PostContentResult>> GetPostsByPoster()
        {
            return await GetPostsByPoster(GetUserID()!);
        }

        [HttpGet("List/{parentPostId}")]
        public async Task<ActionResult<List<PostContentResult>>> GetPostsByParent(int? parentPostId)
        {
            return Ok(await GetPosts(parentPostId == null ? "ParentPostId IS NULL" : $"ParentPostId = {parentPostId}"));
        }

        [HttpGet("List")]
        public async Task<ActionResult<List<PostContentResult>>> GetPostsByParent()
        {
            return await GetPostsByParent(null);
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
