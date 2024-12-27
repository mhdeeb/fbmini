using Microsoft.AspNetCore.Identity;

namespace fbmini.Server.Models
{
    public class UserView()
    {
        public string? UserName { get; set; }
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public IFormFile? Picture { get; set; }
        public IFormFile? Cover { get; set; }
        public bool? IsOwner { get; set; }
    }

    public class User : IdentityUser
    {
        public int UserDataId { get; set; }
        public UserData UserData { get; set; } = null!;

        public ICollection<PostModel> LikedPosts { get; set; } = [];
        public ICollection<PostModel> DislikedPosts { get; set; } = [];

        public UserView ToView()
        {
            return new UserView
            {
                UserName = UserName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Bio = UserData.Bio,
                Id = Id,
                Picture = UserData.Picture?.ToFormFile(),
                Cover = UserData.Cover?.ToFormFile(),
            };
        }
    }
}
