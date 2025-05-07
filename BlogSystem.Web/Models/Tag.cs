using System.ComponentModel.DataAnnotations;

namespace BlogSystem.Web.Models
{
    public class Tag
    {
        public Tag()
        {
            BlogPosts = new HashSet<BlogPost>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<BlogPost> BlogPosts { get; set; }
    }
} 