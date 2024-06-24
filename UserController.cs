using CST_323_MilestoneApp.Models;
using CST_323_MilestoneApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CST_323_MilestoneApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserDAO _userDAO;

        public UserController(UserDAO userDAO)
        {
            _userDAO = userDAO;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _userDAO.GetAllUsersAsync();
            return View(users);
        }

        // GET: User/Details/{userId}
        public async Task<IActionResult> Details(int userId)
        {
            var user = await _userDAO.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                await _userDAO.AddUserAsync(user);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Edit/{userId}
        public async Task<IActionResult> Edit(int userId)
        {
            var user = await _userDAO.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Edit/{userId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int userId, [Bind("UserId,Username,Email,Password")] User user)
        {
            if (userId != user.user_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _userDAO.UpdateUserAsync(user);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Delete/{userId}
        public async Task<IActionResult> Delete(int userId)
        {
            var user = await _userDAO.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/{userId}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int userId)
        {
            await _userDAO.DeleteUserAsync(userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
