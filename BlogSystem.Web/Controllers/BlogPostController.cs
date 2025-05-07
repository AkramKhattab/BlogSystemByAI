using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BlogSystem.Web.Data;
using BlogSystem.Web.Models;
using BlogSystem.Web.ViewModels;
using System.Security.Claims;

namespace BlogSystem.Web.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogPostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BlogPost
        public async Task<IActionResult> Index(string searchString, string category, string tag)
        {
            var posts = _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Where(p => p.Status == PostStatus.Published);

            if (!string.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(p => p.Title.Contains(searchString) || p.Content.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(category))
            {
                posts = posts.Where(p => p.Category.Name == category);
            }

            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(p => p.Tags.Any(t => t.Name == tag));
            }

            var viewModel = new BlogPostListViewModel
            {
                Posts = await posts.OrderByDescending(p => p.CreatedAt).ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync()
            };

            return View(viewModel);
        }

        // GET: BlogPost/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Include(p => p.Comments.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: BlogPost/Create
        [Authorize(Roles = "Admin,Editor")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.Tags = new MultiSelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: BlogPost/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> Create(BlogPostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var post = new BlogPost
                {
                    Title = model.Title,
                    Content = model.Content,
                    AuthorId = userId,
                    CategoryId = model.CategoryId,
                    Status = model.Status,
                    CreatedAt = DateTime.UtcNow
                };

                if (model.SelectedTagIds != null)
                {
                    var tags = await _context.Tags
                        .Where(t => model.SelectedTagIds.Contains(t.Id))
                        .ToListAsync();
                    post.Tags = tags;
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.Tags = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(model);
        }

        // GET: BlogPost/Edit/5
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.BlogPosts
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            // Check if user is authorized to edit this post
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && post.AuthorId != userId)
            {
                return Forbid();
            }

            var viewModel = new BlogPostEditViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CategoryId = post.CategoryId,
                Status = post.Status,
                SelectedTagIds = post.Tags.Select(t => t.Id).ToList()
            };

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.Tags = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(viewModel);
        }

        // POST: BlogPost/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> Edit(int id, BlogPostEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var post = await _context.BlogPosts
                        .Include(p => p.Tags)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (post == null)
                    {
                        return NotFound();
                    }

                    // Check if user is authorized to edit this post
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var isAdmin = User.IsInRole("Admin");
                    if (!isAdmin && post.AuthorId != userId)
                    {
                        return Forbid();
                    }

                    post.Title = model.Title;
                    post.Content = model.Content;
                    post.CategoryId = model.CategoryId;
                    post.Status = model.Status;
                    post.UpdatedAt = DateTime.UtcNow;

                    // Update tags
                    post.Tags.Clear();
                    if (model.SelectedTagIds != null)
                    {
                        var tags = await _context.Tags
                            .Where(t => model.SelectedTagIds.Contains(t.Id))
                            .ToListAsync();
                        post.Tags = tags;
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.Tags = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(model);
        }

        // POST: BlogPost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post != null)
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
} 