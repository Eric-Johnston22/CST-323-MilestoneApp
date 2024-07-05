using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CST_323_MilestoneApp.Models;
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
        public async Task<IActionResult> Index()
        {
            using (_logger.LogMethodEntry())
            {
                _logger.LogInformation("Retrieving authors from database");
         
                var authors = await _authorDAO.GetAllAuthorsAsync(); // call DAO

                return View(authors);
            }
        }

        // GET: /Author/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            using (_logger.LogMethodEntry(nameof(Details), id))
            {
                if (id == null)
                {
                    _logger.LogWarning("Author not found, ID is null");
                    return NotFound();
                }

                var author = await _authorDAO.GetAuthorById(Convert.ToInt32(id)); // call DAO
                return View(author);
            }
        }

    }
}
