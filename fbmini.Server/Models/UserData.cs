using System.ComponentModel.DataAnnotations;

namespace fbmini.Server.Models
{
    public class UserData
    {
        [Key]
        public int Id { get; set; }
        public string? Bio { get; set; }

        public int? PictureId { get; set; }
        public FileModel? Picture { get; set; }

        public int? CoverId { get; set; }
        public FileModel? Cover { get; set; }
    }
}
