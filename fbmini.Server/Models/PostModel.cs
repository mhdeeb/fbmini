using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace fbmini.Server.Models
{
    public class PostForm
    {
        public required string Title { get; set; }
        public string? Content { get; set; }
        public IFormFile? Attachment { get; set; }
    };

    public class PostContentResult
    {
        public int? Id { get; set; }
        public required string Title { get; set; }
        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime? Date { get; set; }
        public int? Likes { get; set; }
        public int? Dislikes { get; set; }
        public List<int>? SubPostsIds { get; set; }
        public UserContentResult? Poster { get; set; }
        public bool? CanEdit { get; set; }
    };

    public class PostModel
    {
        [Key]
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Content { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public string PosterId { get; set; } = null!;
        [Required]
        [ForeignKey("PosterId")]
        public UserModel Poster { get; set; } = null!;
        public int? ParentPostId { get; set; }
        [JsonIgnore]
        [ForeignKey("ParentPostId")]
        public PostModel? ParentPost { get; set; }

        public int? AttachmentId { get; set; }
        [ForeignKey("AttachmentId")]
        public FileModel? Attachment { get; set; }
        public ICollection<UserModel> Likers { get; } = [];
        public ICollection<UserModel> Dislikers { get; } = [];
        public ICollection<PostModel> SubPosts { get; } = [];

        public PostContentResult ToContentResult(bool canEdit)
        {
            return new PostContentResult
            {
                Id = Id,
                Title = Title,
                Content = Content,
                AttachmentUrl = Attachment?.GetUrl(),
                Date = Date,
                Likes = Likers.Count,
                Dislikes = Dislikers.Count,
                SubPostsIds = SubPosts.Select(p => p.Id).ToList(),
                Poster = new UserContentResult { UserName = Poster.UserName, PictureUrl = Poster?.UserData?.Picture?.GetUrl() },
                CanEdit = canEdit
            };
        }

        public static async Task<PostModel> FromFormAsync(PostForm postForm, string posterId)
        {
            var post = new PostModel
            {
                PosterId = posterId,
                Title = postForm.Title,
                Content = postForm.Content
            };

            if (postForm.Attachment != null)
            {
                using var stream = new MemoryStream();
                await postForm.Attachment.CopyToAsync(stream);

                post.Attachment = new FileModel
                {
                    FileName = postForm.Attachment.FileName,
                    ContentType = postForm.Attachment.ContentType,
                    FileData = stream.ToArray(),
                    OwnerId = posterId,
                    AccessType = AccessType.Public,
                };
            }

            return post;
        }
    }
}
