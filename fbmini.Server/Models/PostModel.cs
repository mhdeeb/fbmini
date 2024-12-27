using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace fbmini.Server.Models
{
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
    }
}
