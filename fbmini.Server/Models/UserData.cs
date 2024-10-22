using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fbmini.Server.Models
{
    public class UserData
    {
        [Key]
        public int Id { get; set; }
        public string? Bio { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public int? PictureId { get; set; }
        [ForeignKey("PictureId")]
        public FileModel? Picture { get; set; }
        public int? CoverId { get; set; }
        [ForeignKey("CoverId")]
        public FileModel? Cover { get; set; }
        public ICollection<PostModel> Posts { get; } = [];
    }
}
