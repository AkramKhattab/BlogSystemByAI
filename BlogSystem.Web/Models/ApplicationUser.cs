using Microsoft.AspNetCore.Identity;

namespace BlogSystem.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
} 