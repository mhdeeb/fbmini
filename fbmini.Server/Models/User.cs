using Microsoft.AspNetCore.Identity;

namespace fbmini.Server.Models
{
    public class User : IdentityUser
    {
        public int UserDataId { get; set; }
        public virtual UserData? UserData { get; set; }
    }
}
