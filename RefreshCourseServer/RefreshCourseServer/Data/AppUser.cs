using Microsoft.AspNetCore.Identity;

namespace RefreshCourseServer.Data
{
    public class AppUser : IdentityUser
    {
        public string? PublicKey { get; set; }
    }
}