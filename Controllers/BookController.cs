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

            return View();
        }
    }
}
