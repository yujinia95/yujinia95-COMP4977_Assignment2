using Microsoft.AspNetCore.Identity;

namespace IosAssignment2Backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        // Automatically set when the user is created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Manually set when login succeeds
        public DateTime? LastLoginDate { get; set; }
    }
}