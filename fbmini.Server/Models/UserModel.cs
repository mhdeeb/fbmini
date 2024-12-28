using Microsoft.AspNetCore.Identity;

namespace fbmini.Server.Models
{
    public class UserForm()
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public IFormFile? Picture { get; set; }
        public IFormFile? Cover { get; set; }
    }

    public class UserContentResult()
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? PictureUrl { get; set; }
        public string? CoverUrl { get; set; }
        public bool? IsOwner { get; set; }
    }

    public class UserModel : IdentityUser
    {
        public int UserDataId { get; set; }
        public UserDataModel UserData { get; set; } = null!;

        public ICollection<PostModel> LikedPosts { get; set; } = [];
        public ICollection<PostModel> DislikedPosts { get; set; } = [];

        public UserContentResult ToContentResult()
        {
            return new UserContentResult
            {
                UserName = UserName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Bio = UserData.Bio,
                PictureUrl = UserData.Picture?.GetUrl(),
                CoverUrl = UserData.Cover?.GetUrl(),
            };
        }
    }
}
