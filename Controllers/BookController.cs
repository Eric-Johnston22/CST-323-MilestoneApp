using CST_323_MilestoneApp.Services;
using CST_323_MilestoneApp.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        // Retrieves a list of all books
        public async Task<IActionResult> Index()
        {
            using (_logger.LogMethodEntry())
            {
                _logger.LogInformationWithContext("Retrieving all books from database");
                var books = await _bookDAO.GetAllBooksAsync(); // Fetch all books from the DAO

                return View(books); // Pass the list of books to the view
            }
        }

        // GET: book/details/{id}
        // Retrieves details of a specific book by ID
        public async Task<IActionResult> Details(int? id)
        {
            using (_logger.LogMethodEntry(nameof(Details), id))
            {
                if (id == null)
                {
                    _logger.LogWarningWithContext("Book not found, ID is null");
                    return NotFound(); // Handle the case where the book is not found
                }

                var book = await _bookDAO.GetBookByIdAsync(Convert.ToInt32(id)); // Fetch book details from the DAO
                return View(book); // Pass the book details to the view
            }
        }

        // POST: book/addtowanttoread
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWantToRead(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToWantToRead), bookId))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // Get the logged-in user's ID
                bool result = await _userDAO.AddToWantToReadAsync(userId, bookId); // Check if the book is already in the user's list

                if (!result) 
                {
                    _logger.LogWarningWithContext($"User {@userId} already wants to read {@bookId}");
                    var book = _bookDAO.GetBookByIdAsync(bookId);

                    return Json(new { success = false, message = $"{book.Result.Title} is already on one of your lists." }); // return json message
                }
                else
                {
                    _logger.LogInformationWithContext($"User {@userId} does not have {@bookId} on their Want To Read list");
                    await _userDAO.AddToWantToReadAsync(userId, bookId); // Add the book to the user's "Want to Read" list
                    var book = _bookDAO.GetBookByIdAsync(bookId);
                    return Json(new { success = true, message = $"{book.Result.Title} added to your Want to Read list." }); // return json message
                }

            }
        }

        // POST: book/addtocurrentlyreading
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCurrentlyReading(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToCurrentlyReading), bookId))
            {
                _logger.LogInformationWithContext($"Checking if book {bookId} is already in 'CurrentlyReading' list");
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // Get the logged-in user's ID
                bool result = await _userDAO.AddToCurrentlyReadingAsync(userId, bookId); // Check if the book is already in the user's list

                if (!result)
                {
                    _logger.LogWarningWithContext($"User {@userId} is currently reading {@bookId}");
                    var book = _bookDAO.GetBookByIdAsync(bookId); 
                  
                    return Json(new { success = false, message = $"{book.Result.Title} is already on one of your lists." }); // return json message
                }
                else
                {
                    _logger.LogInformationWithContext($"User {@userId} is NOT currently reading {@bookId}");
                    await _userDAO.AddToCurrentlyReadingAsync(userId, bookId); // Add the book to the user's "Currently Reading" list
                    var book = _bookDAO.GetBookByIdAsync(bookId);
                    return Json(new { success = true, message = $"{book.Result.Title} added to your Currently Reading list." }); // return json message
                }
            }
        }

        // POST: book/addtohaveread
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToHaveRead(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToHaveRead), bookId))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // Get the logged-in user's ID
                bool result = await _userDAO.AddToHaveReadAsync(userId, bookId); // Check if the book is already in the user's list

                if (!result)
                {
                    _logger.LogWarningWithContext($"User {@userId} already has {@bookId} on their Have Read list");
                    var book = _bookDAO.GetBookByIdAsync(bookId);

                    return Json(new { success = false, message = $"{book.Result.Title} is already on one of your lists." }); // return json message
                }
                else
                {
                    _logger.LogInformationWithContext($"User {@userId} has not yet read {@bookId}");
                    await _userDAO.AddToHaveReadAsync(userId, bookId); // Add the book to the user's "Have Read" list
                    var book = _bookDAO.GetBookByIdAsync(bookId);

                    return Json(new { success = true, message = $"{book.Result.Title} added to your Have Read list." }); // return json message
                }
            }
        }
    }
}
