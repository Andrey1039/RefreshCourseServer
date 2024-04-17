using System.ComponentModel.DataAnnotations;

namespace RefreshCourseServer.Models
{
    // Модель регистрации на сервере
    public class RegisterModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Initials { get; set; }
    }
}