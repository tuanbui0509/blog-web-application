using Microsoft.AspNetCore.Identity;

namespace BlogWeb.Domain.Entities.Authentication
{
    public class UserApplication : IdentityUser
    {
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}