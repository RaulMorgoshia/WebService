using System.ComponentModel.DataAnnotations;

namespace WebService.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }


     public class Image
    {
        public int Id { get; set; }
        public string? FileName { get; set; }  // Name of the image file (e.g., "profile.jpg")
        public string? FilePath { get; set; }  // Path to the image (e.g., "/images/profile.jpg")
        public string? ContentType { get; set; } // MIME type of the image (e.g., "image/jpeg")
        public int UserId { get; set; } // Foreign key to the User
        [Required]
        public User? User { get; set; }  // Navigation property to the User
    }
}
