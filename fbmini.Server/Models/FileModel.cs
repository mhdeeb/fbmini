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
        public string? ContentType { get; set; }

        public long Size { get; set; }

        public byte[]? FileData { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    }
}