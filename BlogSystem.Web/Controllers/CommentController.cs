using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BlogSystem.Web.Data;
using BlogSystem.Web.Models;
using System.Security.Claims;

namespace BlogSystem.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(int postId, string content, int? parentCommentId = null)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Comment content cannot be empty.");
            }

            var post = await _context.BlogPosts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = new Comment
            {
                Content = content,
                PostId = postId,
                AuthorId = userId,
                ParentCommentId = parentCommentId,
                CreatedAt = DateTime.UtcNow,
                IsApproved = User.IsInRole("Admin") || User.IsInRole("Editor")
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "BlogPost", new { id = postId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "BlogPost", new { id = comment.PostId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.IsApproved = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "BlogPost", new { id = comment.PostId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Comment content cannot be empty.");
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.AuthorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            comment.Content = content;
            comment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "BlogPost", new { id = comment.PostId });
        }
    }
} 