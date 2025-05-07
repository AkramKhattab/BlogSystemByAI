using BlogSystem.Web.Models;

namespace BlogSystem.Web.ViewModels
{
    public class BlogPostListViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
} 