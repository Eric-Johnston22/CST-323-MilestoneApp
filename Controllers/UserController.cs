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

        // Constructor to initialize DAOs and logger
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
                    // Check if password matches confirmation password
                    if (user.Password != user.ConfirmPassword)
                    {
                        _logger.LogWarningWithContext("Password and confirmation password do not match");
                        ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
                        return View(user);
                    }

                    var result = await _userDAO.RegisterUserAsync(user); // Register the user
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
                var loggedInUser = await _userDAO.AuthenticateUserAsync(user.Username, user.Password); // Authenticate user

                if (loggedInUser != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, loggedInUser.User_id.ToString()),
                        new Claim(ClaimTypes.Name, loggedInUser.Username)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)); // Sign in user

                    _logger.LogInformationWithContext("User logged in successfully");
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
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get user ID
                _logger.LogInformationWithContext($"User {userId} logging out");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Sign out user
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: User/Profile/{id}
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            using (_logger.LogMethodEntry(nameof(Profile), id))
            {
                var user = await _userDAO.GetUserByIdAsync(id); // Get user by ID
                if (user == null)
                {
                    return NotFound(); // Handle the case where the user is not found
                }

                if (User.Identity.IsAuthenticated)
                {
                    // If the user is authenticated, pass user's ID to the view
                    var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    ViewBag.LoggedInUserId = loggedInUserId;
                }
                else
                {
                    // If not, return null to the view
                    ViewBag.LoggedInUserId = null;
                }

                return View(user);
            }
        }

        // POST: User/FinishReading
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FinishReading(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(FinishReading), bookId))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // Get logged-in user's ID
                await _userDAO.FinishReadingBookAsync(userId, bookId); // Mark book as finished reading
                var book = await _bookDAO.GetBookByIdAsync(bookId); // Get book details
                if (book == null)
                {
                    _logger.LogError($"Book not found by ID: {bookId}");
                    return NotFound();
                }
                _logger.LogInformationWithContext($"Book added to Finished Reading List: {bookId}");
                var review = new Review { Book_id = bookId, Book = book }; // Create a new review object
                return View("WriteReview", review); // Redirect to write review view
            }
        }

        // GET: User/WriteReview/{bookId}
        [Authorize]
        public async Task<IActionResult> WriteReview(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(WriteReview), bookId))
            {
                var book = await _bookDAO.GetBookByIdAsync(bookId); // Get book details
                if (book == null)
                {
                    _logger.LogWarningWithContext($"Book not found by ID: {bookId}");
                    return NotFound();
                }
                var review = new Review { Book_id = bookId, Book = book }; // Create a new review object
                _logger.LogInformationWithContext($"Retrieved review {review.Review_id} for book {bookId} from database");
                return View(review); // Return the review view
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
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // Get logged-in user's ID
                    review.User_id = userId;
                    review.Review_date = DateTime.Now;

                    // Fetch the Book object to ensure it is correctly populated
                    review.Book = await _bookDAO.GetBookByIdAsync(review.Book_id);
                    review.User = await _userDAO.GetUserByIdAsync(userId);

                    _logger.LogInformationWithContext($"Saving Review: {review.Review_id} to database");

                    await _userDAO.AddReviewAsync(review); // Add the review to the database

                    return RedirectToAction("Profile", new { id = userId });
                }

                // Populate the Book property before returning the view
                review.Book = await _bookDAO.GetBookByIdAsync(review.Book_id);
                return View(review);
            }
        }

        // GET: User/ReviewDetails/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReviewDetails(int id)
        {
            using (_logger.LogMethodEntry(nameof(ReviewDetails), id))
            {
                _logger.LogInformationWithContext($"Retrieved review {id} from database");
                var review = await _userDAO.GetReviewById(id); // Get review details by ID
                if (review == null)
                {
                    _logger.LogError("Review is null");
                    return NotFound();
                }

                return View(review); // Return the review details view
            }
        }

        // POST: User/MoveToCurrentlyReading
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MoveToCurrentlyReading(int bookId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // Get logged-in user's ID
            var result = await _userDAO.MoveToCurrentlyReadingAsync(userId, bookId); // Move the book to Currently Reading list

            if (result)
            {
                var book = await _bookDAO.GetBookByIdAsync(bookId); // Get book details
                return Json(new { success = true, message = "Book moved to Currently Reading list.", bookTitle = book.Title }); // Return success message
            }
            else
            {
                return Json(new { success = false, message = "Book is already in Currently Reading or Have Read list." }); // Return failure message
            }
        }
    }
}

