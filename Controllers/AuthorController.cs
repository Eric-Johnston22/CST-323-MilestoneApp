using Microsoft.AspNetCore.Mvc;
using CST_323_MilestoneApp.Services;
using CST_323_MilestoneApp.Utilities;

namespace CST_323_MilestoneApp.Controllers
{
    public class AuthorController : Controller
    {

        private readonly AuthorDAO _authorDAO;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(AuthorDAO authorDAO, ILogger<AuthorController> logger)
        {
            _authorDAO = authorDAO;
            _logger = logger;
        }

        // GET: /Author
        // Retrieves a list of all authors
        public async Task<IActionResult> Index()
        {
            using (_logger.LogMethodEntry())
            {
                _logger.LogInformationWithContext("Retrieving authors from database");
         
                var authors = await _authorDAO.GetAllAuthorsAsync(); // Fetch all authors from the DAO

                return View(authors); // Pass the list of authors to the view
            }
        }

        // GET: /Author/Details/{id}
        // Retrieves details of a specific author by ID
        public async Task<IActionResult> Details(int? id)
        {
            using (_logger.LogMethodEntry(nameof(Details), id))
            {
                if (id == null)
                {
                    _logger.LogWarningWithContext("Author not found, ID is null");
                    return NotFound(); // Return 404 if ID is null
                }

                var author = await _authorDAO.GetAuthorById(Convert.ToInt32(id)); // Fetch author details from the DAO
                return View(author); // Pass the author details to the view

            }
        }

    }
}
