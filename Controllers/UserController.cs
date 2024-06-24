using CST_323_MilestoneApp.Models;
using CST_323_MilestoneApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CST_323_MilestoneApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserDAO _userDAO;
        private readonly ILogger<UserController> _logger;

        public UserController(UserDAO userDAO, ILogger<UserController> logger)
        {
            _userDAO = userDAO;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: User/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (user == null)
            {
                _logger.LogError("User object is null.");
                return BadRequest("User object is null.");
            }

            if (ModelState.IsValid)
            {
                if (user.Password != user.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
                    return View(user);
                }

                var result = await _userDAO.RegisterUserAsync(user);
                if (result)
                {
                    _logger.LogInformation("User registered successfully.");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning("User registration failed.");
                    ModelState.AddModelError(string.Empty, "User registration failed.");
                }
            }
            return View(user);
        }

        // GET: User/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            var loggedInUser = await _userDAO.AuthenticateUserAsync(user.Username, user.Password);

            if (loggedInUser != null)
            {
                // Redirect to the profile page
                return RedirectToAction("Profile", new { id = loggedInUser.User_id });
            }

            ModelState.AddModelError("", "Invalid username or password");
            ViewBag.ErrorMessage = "Invalid username or password";
            return View(user);
        }

        // GET: User/Profile/{id}
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var user = await _userDAO.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return NotFound(); // Handle the case where the user is not found
            }

            return View(user);
        }
    }
}
