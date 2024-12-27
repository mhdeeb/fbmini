using System.ComponentModel.DataAnnotations;

namespace fbmini.Server.Models
{
    public class FileModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string? FileName { get; set; }

        [StringLength(100)]
        public required string ContentType { get; set; }

        [Required]
        public long Size { get; set; }

        public required byte[] FileData { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    }
}