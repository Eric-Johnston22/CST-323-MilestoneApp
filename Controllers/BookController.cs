using CST_323_MilestoneApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CST_323_MilestoneApp.Controllers
{
    public class BookController : Controller
    {
        private readonly BookDAO _bookDAO;
        private readonly UserDAO _userDAO;

        // Constructor
        public BookController(BookDAO bookDAO, UserDAO userDAO)
        {
            _bookDAO = bookDAO;
            _userDAO = userDAO;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var books = await _bookDAO.GetAllBooksAsync();

            return View(books);
        }

        // GET: book/details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookDAO.GetBookByIdAsync(id);
            if (book == null)
            {
                
                return NotFound(); // Handle the case where the book is not found
            }
            return View(book);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWantToRead(int bookId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _userDAO.AddToWantToReadAsync(userId, bookId);
            return RedirectToAction("Details", new { id = bookId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCurrentlyReading(int bookId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _userDAO.AddToCurrentlyReadingAsync(userId, bookId);
            return RedirectToAction("Details", new { id = bookId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToHaveRead(int bookId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _userDAO.AddToHaveReadAsync(userId, bookId);
            return RedirectToAction("Details", new { id = bookId });
        }
    }
}
