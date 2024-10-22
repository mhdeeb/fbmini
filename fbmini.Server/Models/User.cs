using Microsoft.AspNetCore.Identity;

namespace fbmini.Server.Models
{
    public class User : IdentityUser
    {
        public int UserDataId { get; set; }
        public UserData UserData { get; set; } = null!;

        public ICollection<PostModel> LikedPosts { get; set; } = [];
        public ICollection<PostModel> DislikedPosts { get; set; } = [];
    }
}
