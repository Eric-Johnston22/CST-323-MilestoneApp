using CST_323_MilestoneApp.Controllers;
using CST_323_MilestoneApp.Models;
using CST_323_MilestoneApp.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CST_323_MilestoneApp.Services
{
    public class UserDAO
    {
        private readonly LibraryContext _context;
        private readonly ILogger<UserDAO> _logger;

        public UserDAO(LibraryContext context, ILogger<UserDAO> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            using (_logger.LogMethodEntry(nameof(RegisterUserAsync), user))
            {
                try
                {
                    // Ensure the username is unique
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                    if (existingUser != null)
                    {
                        _logger.LogWarning("Username already exists.");
                        return false;
                    }

                    // Hash the password before saving (optional, not implemented here for simplicity)
                    // user.Password = HashPassword(user.Password);

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Saving user {id} to database", user.User_id);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while registering user.");
                    return false;
                }
            }
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            using (_logger.LogMethodEntry(nameof(AuthenticateUserAsync), username, password))
            {
                try
                {
                    var user = await _context.Users
                                             .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
                    if (user == null)
                    {
                        _logger.LogWarning("Invalid username or password.");
                        return user;
                    }

                    _logger.LogInformation("User retrieved from database");
                    return user;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while authenticating user.");
                    throw;
                }
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            using (_logger.LogMethodEntry(nameof(GetUserByIdAsync), userId))
            {
                try
                {
                    var user = await _context.Users
                                             .Include(u => u.CurrentlyReading)
                                             .ThenInclude(cr => cr.Book)
                                             .Include(u => u.ReadingHistory)
                                             .ThenInclude(rh => rh.Book)
                                             .Include(u => u.WantToRead)
                                             .ThenInclude(w => w.Book)
                                             .Include(u => u.Review)
                                             .ThenInclude(r => r.Book)
                                             .FirstOrDefaultAsync(u => u.User_id == userId);
                    if (user == null)
                    {
                        _logger.LogWarning("User with ID {userId} not found.", userId);
                    }

                    _logger.LogInformation("User {userid} retrieved from database", userId);
                    return user;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching user with ID {userId}.", userId);
                    throw;
                }
            }
        }

        public async Task AddToWantToReadAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToWantToReadAsync), userId, bookId))
            {
                var wantToRead = new WantToRead { User_id = userId, Book_id = bookId };
                _context.WantToRead.Add(wantToRead);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saving book {id} to user {id}'s WantToRead list", bookId, userId);
            }
        }

        public async Task AddToCurrentlyReadingAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToCurrentlyReadingAsync), userId, bookId))
            {
                var currentlyReading = new CurrentlyReading { User_id = userId, Book_id = bookId, Start_date = DateTime.Now };
                _context.CurrentlyReading.Add(currentlyReading);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saving book {id} to user {id}'s CurrentlyReading list", bookId, userId);
            }
        }

        // Check to see if user is already reading the book
        public async Task<bool> IsBookInCurrentlyReadingAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(IsBookInCurrentlyReadingAsync), userId, bookId))
            {
                return await _context.CurrentlyReading.AnyAsync(cr => cr.Book_id == bookId && cr.User_id == userId);
            }
        }

        public async Task AddToHaveReadAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToHaveReadAsync), userId, bookId))
            {
                var readingHistory = new ReadingHistory { User_id = userId, Book_id = bookId, Start_date = DateTime.Now, Finish_date = DateTime.Now };
                _context.ReadingHistory.Add(readingHistory);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saving book {id} to user {id}'s 'HaveRead' list", bookId, userId);
            }
        }

        public async Task FinishReadingBookAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(FinishReadingBookAsync), userId, bookId))
            {
                var currentlyReading = await _context.CurrentlyReading.FirstOrDefaultAsync(cr => cr.User_id == userId && cr.Book_id == bookId);
                if (currentlyReading != null)
                {
                    _logger.LogInformation("Removing book {id} from user {id}'s 'CurrentlyReading' list", bookId, userId);
                    _context.CurrentlyReading.Remove(currentlyReading);

                    var readingHistory = new ReadingHistory
                    {
                        User_id = userId,
                        Book_id = bookId,
                        Start_date = currentlyReading.Start_date,
                        Finish_date = DateTime.Now
                    };
                    _context.ReadingHistory.Add(readingHistory);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Saving book {id} to user {id}'s Reading History", bookId, userId);
                }
            }
        }

        public async Task AddReviewAsync(Review review)
        {
            using (_logger.LogMethodEntry(nameof(AddReviewAsync), review))
            {
                _logger.LogInformation("Attempting to add review: {@Review}", review);

                // Ensure that the User and Book objects are attached to the context
                _context.Entry(review.User).State = EntityState.Unchanged;
                _context.Entry(review.Book).State = EntityState.Unchanged;

                _context.Reviews.Add(review);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Review {id} saved to database", review.Review_id);
            }
        }

        public async Task<Review?> GetReviewById(int reviewId)
        {
            using (_logger.LogMethodEntry(nameof(GetReviewById), reviewId))
            {
                _logger.LogInformation("Retrieving review {id} from database", reviewId);
                var review = await _context.Reviews
                    .Include(r => r.Book)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Review_id == reviewId);

                return review;
            }
        }
    }
}
