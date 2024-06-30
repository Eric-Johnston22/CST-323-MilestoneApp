using CST_323_MilestoneApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CST_323_MilestoneApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LibraryContext _context;

        public HomeController(ILogger<HomeController> logger, LibraryContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index action called.");
            _logger.LogWarning("WARNING LOG TEST FROM HOME CONTROLLER");
            _logger.LogError("ERROR LOG TEST FROM HOME CONTROLLER");
            _logger.LogTrace("TRACE LOG TEST FROM HOME CONTROLLER");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("SearchResults", new List<Book>());
            }

            var books = await _context.Books
                .Include(b => b.Author)
                .Where(b => b.Title.Contains(query) || b.Genre.Contains(query) || b.ISBN.Contains(query) || b.Author.Name.Contains(query))
                .ToListAsync();

            return View("SearchResults", books);
        }
    }
}
