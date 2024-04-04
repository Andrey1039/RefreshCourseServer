using System.ComponentModel.DataAnnotations;

namespace RefreshCourseServer.Models
{
    public class SwapModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? PublicKey { get; set; }
    }
}
