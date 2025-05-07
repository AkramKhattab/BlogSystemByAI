using System.ComponentModel.DataAnnotations;
using BlogSystem.Web.Models;

namespace BlogSystem.Web.ViewModels
{
    public class BlogPostEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Tags")]
        public List<int> SelectedTagIds { get; set; }

        [Display(Name = "Status")]
        public PostStatus Status { get; set; }
    }
} 