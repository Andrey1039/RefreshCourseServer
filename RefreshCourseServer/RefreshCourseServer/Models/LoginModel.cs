using System.ComponentModel.DataAnnotations;

namespace RefreshCourseServer.Models
{
    // Модель авторизации на сервере
    public class LoginModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
