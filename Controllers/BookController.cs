using CST_323_MilestoneApp.Services;
using CST_323_MilestoneApp.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CST_323_MilestoneApp.Controllers
{
    public class BookController : Controller
    {
        private readonly BookDAO _bookDAO;
        private readonly UserDAO _userDAO;
        private readonly ILogger<BookController> _logger;

        // Constructor
        public BookController(BookDAO bookDAO, UserDAO userDAO, ILogger<BookController> logger)
        {
            _bookDAO = bookDAO;
            _userDAO = userDAO;
            _logger = logger;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            using (_logger.LogMethodEntry())
            {
                _logger.LogInformationWithContext("Retrieving all books from database");
                var books = await _bookDAO.GetAllBooksAsync();

                return View(books);
            }
        }

        // GET: book/details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            using (_logger.LogMethodEntry(nameof(Details), id))
            {
                if (id == null)
                {
                    _logger.LogWarningWithContext("Book not found, ID is null");
                    return NotFound(); // Handle the case where the book is not found
                }

                var book = await _bookDAO.GetBookByIdAsync(Convert.ToInt32(id)); // Call DAO
                return View(book);
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWantToRead(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToWantToRead), bookId))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _userDAO.AddToWantToReadAsync(userId, bookId);
                TempData["SuccessMessage"] = $"Book added successfully to Want to Read list.";
                return NoContent();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCurrentlyReading(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToCurrentlyReading), bookId))
            {
                _logger.LogInformationWithContext($"Checking if book {bookId} is already in 'CurrentlyReading' list");
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                bool alreadyReading = await _userDAO.IsBookInCurrentlyReadingAsync(userId, bookId);

                if (alreadyReading)
                {
                    _logger.LogWarningWithContext($"User {@userId} is currently reading {@bookId}");
                    var book = _bookDAO.GetBookByIdAsync(bookId);
                    //TempData["ErrorMessage"] = $"You are already reading {book.Result.Title}!";
                    return Json(new { success = false, message = $"{book.Result.Title} is already on your CurrentlyReading list." });
                }
                else
                {
                    _logger.LogInformationWithContext($"User {@userId} is NOT currently reading {@bookId}");
                    await _userDAO.AddToCurrentlyReadingAsync(userId, bookId);
                    var book = _bookDAO.GetBookByIdAsync(bookId);
                    //TempData["SuccessMessage"] = $"Book added successfully to Currently Reading list.";
                    return Json(new { success = true, message = $"{book.Result.Title} added to your Currently Reading list." });
                }
                //return RedirectToAction("Details", new { id = bookId });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToHaveRead(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToHaveRead), bookId))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _userDAO.AddToHaveReadAsync(userId, bookId);
                TempData["SuccessMessage"] = $"Book added successfully to Have Read list.";
                return RedirectToAction("Details", new { id = bookId });
            }
        }
    }
}
