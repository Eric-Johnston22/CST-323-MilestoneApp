using CST_323_MilestoneApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CST_323_MilestoneApp.Controllers
{
    public class BookController : Controller
    {
        private readonly BookDAO _bookDAO;

        // Constructor
        public BookController(BookDAO bookDAO)
        {
            _bookDAO = bookDAO;
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
    }
}
