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
        [Required]
        public required string ContentType { get; set; }

        [Required]
        public required byte[] FileData { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public IFormFile ToFormFile()
        {
            var stream = new MemoryStream(FileData);
            var formFile = new FormFile(stream, 0, FileData.Length, "file", FileName ?? "")
            {
                Headers = new HeaderDictionary(),
                ContentType = ContentType
            };
            return formFile;
        }
    }
}