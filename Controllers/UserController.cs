using CST_323_MilestoneApp.Models;
using CST_323_MilestoneApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CST_323_MilestoneApp.Utilities;

namespace CST_323_MilestoneApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserDAO _userDAO;
        private readonly BookDAO _bookDAO;
        private readonly ILogger<UserController> _logger;

        public UserController(UserDAO userDAO, BookDAO bookDAO, ILogger<UserController> logger)
        {
            _userDAO = userDAO;
            _bookDAO = bookDAO;
            _logger = logger;
        }

        // GET: User/Profile
        [HttpGet]
        public IActionResult Index()
        {
            using (_logger.LogMethodEntry())
            {
                return View();
            }
        }

        // GET: User/Register
        [HttpGet]
        public IActionResult Register()
        {
            using (_logger.LogMethodEntry())
            {
                return View();
            }
        }

        // POST: User/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            using (_logger.LogMethodEntry(nameof(Register), user))
            {

                if (user == null)
                {
                    _logger.LogError("User object is null.");
                    return BadRequest("User object is null.");
                }

                // Remove navigation properties from ModelState validation
                ModelState.Remove(nameof(user.CurrentlyReading));
                ModelState.Remove(nameof(user.ReadingHistory));
                ModelState.Remove(nameof(user.WantToRead));
                ModelState.Remove(nameof(user.Review));

                if (ModelState.IsValid)
                {
                    if (user.Password != user.ConfirmPassword)
                    {
                        _logger.LogWarningWithContext("Password and confirmation password do not match");
                        ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
                        return View(user);
                    }

                    var result = await _userDAO.RegisterUserAsync(user);
                    if (result)
                    {
                        _logger.LogInformationWithContext("User registered successfully.");
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        _logger.LogWarningWithContext("User registration failed.");
                        ModelState.AddModelError(string.Empty, "User registration failed.");
                    }
                }
                return View(user);
            }
        }

        // GET: User/Login
        [HttpGet]
        public IActionResult Login()
        {
            using (_logger.LogMethodEntry())
            {
                return View();
            }
        }

        // POST: User/Login
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            using (_logger.LogMethodEntry(nameof(Login), user))
            {
                var loggedInUser = await _userDAO.AuthenticateUserAsync(user.Username, user.Password);

                if (loggedInUser != null)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, loggedInUser.User_id.ToString()),
                    new Claim(ClaimTypes.Name, loggedInUser.Username)
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    _logger.LogInformationWithContext("User logged in succesfully");
                    return RedirectToAction("Profile", new { id = loggedInUser.User_id });
                }

                ModelState.AddModelError("", "Invalid username or password");
                ViewBag.ErrorMessage = "Invalid username or password";
                return View(user);
            }
        }

        // GET: User/Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            using (_logger.LogMethodEntry())
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformationWithContext($"User {userId} logging out");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: User/Profile/{id}
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            using (_logger.LogMethodEntry(nameof(Profile), id))
            {
                var user = await _userDAO.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(); // Handle the case where the user is not found
                }

                // Pass the logged-in user's ID to the view
                var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                ViewBag.LoggedInUserId = loggedInUserId;

                return View(user);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FinishReading(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(FinishReading), bookId))
            {

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _userDAO.FinishReadingBookAsync(userId, bookId);
                var book = await _bookDAO.GetBookByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogError($"Book not found by ID: {bookId}");
                    return NotFound();
                }
                _logger.LogInformationWithContext($"Book added to Finished Reading List: {bookId}");
                var review = new Review { Book_id = bookId, Book = book };
                return View("WriteReview", review);
            }
        }

        // GET: User/WriteReview/{bookId}
        [Authorize]
        public async Task<IActionResult> WriteReview(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(WriteReview), bookId))
            {
                var book = await _bookDAO.GetBookByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarningWithContext($"Book not found by ID: {bookId}");
                    return NotFound();
                }
                var review = new Review { Book_id = bookId, Book = book };
                _logger.LogInformationWithContext($"Retrived review {review.Review_id} for book {bookId} from database");
                return View(review);
            }
        }

        // POST: User/WriteReview
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> WriteReview(Review review)
        {
            using (_logger.LogMethodEntry(nameof(WriteReview), review))
            {
                // Remove Book and User properties from ModelState validation
                ModelState.Remove("Book");
                ModelState.Remove("User");

                if (ModelState.IsValid)
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    review.User_id = userId;
                    review.Review_date = DateTime.Now;

                    // Fetch the Book object to ensure it is correctly populated
                    review.Book = await _bookDAO.GetBookByIdAsync(review.Book_id);
                    review.User = await _userDAO.GetUserByIdAsync(userId);

                    _logger.LogInformationWithContext($"Saving Review: {review.Review_id} to database");

                    await _userDAO.AddReviewAsync(review);

                    return RedirectToAction("Profile", new { id = userId });
                }

                // Populate the Book property before returning the view
                review.Book = await _bookDAO.GetBookByIdAsync(review.Book_id);
                return View(review);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReviewDetails(int id)
        {
            using (_logger.LogMethodEntry(nameof(ReviewDetails), id))
            {
                _logger.LogInformationWithContext($"Retrieved review {id} from database");
                var review = await _userDAO.GetReviewById(id);
                if (review == null)
                {
                    _logger.LogError("Review is null");
                    return NotFound();
                }

                return View(review);
            }
        }
    }
}
