using CST_323_MilestoneApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Serilog.Context;
using CST_323_MilestoneApp.Utilities;

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
            using (_logger.LogMethodEntry())
            {
                return View();
            }

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
            using (_logger.LogMethodEntry(nameof(Search), query))
            {
                if (string.IsNullOrEmpty(query))
                {
                    _logger.LogInformationWithContext("Search Query is empty or null");
                    return View("SearchResults", new List<Book>());
                }

                var books = await _context.Books
                    .Include(b => b.Author)
                    .Where(b => b.Title.Contains(query) || b.Genre.Contains(query) || b.ISBN.Contains(query) || b.Author.Name.Contains(query))
                    .ToListAsync();
                _logger.LogInformationWithContext($"Retrieved all entries with {query}");
                return View("SearchResults", books);
            }
        }
    }
}
