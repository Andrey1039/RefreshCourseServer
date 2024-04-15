using Microsoft.AspNetCore.Identity;

namespace RefreshCourseServer.Data
{
    // Модель пользователя
    public class AppUser : IdentityUser
    {
        public string? PublicKey { get; set; }
    }
}