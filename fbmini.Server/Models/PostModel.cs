using fbmini.Server.Controllers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace fbmini.Server.Models
{
    public class PostView
    {
        public required string Title { get; set; }
        public string? Content { get; set; }
        public IFormFile? Attachment { get; set; }
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public int? Likes { get; set; }
        public int? Dislikes { get; set; }
        public List<int>? SubPostsIds { get; set; }
        public UserView? Poster { get; set; }
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
        public User Poster { get; set; } = null!;
        public int? ParentPostId { get; set; }
        [JsonIgnore]
        [ForeignKey("ParentPostId")]
        public PostModel? ParentPost { get; set; }

        private int? AttachmentId { get; set; }
        [ForeignKey("AttachmentId")]
        public FileModel? Attachment { get; set; }
        public ICollection<User> Likers { get; } = [];
        public ICollection<User> Dislikers { get; } = [];
        public ICollection<PostModel> SubPosts { get; } = [];

        public PostView ToView()
        {
            return new PostView
            {
                Title =  Title,
                Content =  Content,
                Id =  Id,
                Date =  Date,
                Attachment =  Attachment?.ToFormFile(),
                Likes =  Likers.Count,
                Dislikes =  Dislikers.Count,
                SubPostsIds =  SubPosts.Select(p => p.Id).ToList(),
                Poster = new UserView { UserName = Poster.UserName, Picture = Poster?.UserData?.Picture?.ToFormFile() }
            };
        }

        public static async Task<PostModel> FromViewAsync(PostView postView)
        {
            var post = new PostModel
            {
                PosterId = postView.Poster!.Id!,
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
                    FileData = stream.ToArray()
                };
            }

            return post;
        }
    }
}
