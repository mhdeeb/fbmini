using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace fbmini.Server.Models
{
    public enum AccessType
    {
        Public,
        Private
    }

    public class FileModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string OwnerId { get; set; }

        [Required]
        public required AccessType AccessType { get; set; }

        [StringLength(255)]
        public string? FileName { get; set; }

        [StringLength(100)]
        [Required]
        public required string ContentType { get; set; }

        [Required]
        public required byte[] FileData { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public string GetUrl()
        {
            return $"api/file/blob/{Id}";
        }
        public static string GetUrl(int Id)
        {
            return $"api/file/blob/{Id}";
        }
        public FileContentResult ToContentResult()
        {
            return new FileContentResult(FileData, ContentType);
        }

        public static async Task<FileModel> FromFormAsync(IFormFile formFile, string ownerId)
        {
            using var stream = new MemoryStream();
            await formFile.CopyToAsync(stream);

            return new FileModel
            {
                FileName = formFile.FileName,
                ContentType = formFile.ContentType,
                FileData = stream.ToArray(),
                AccessType = AccessType.Public,
                OwnerId = ownerId,
            };
        }
    }
}