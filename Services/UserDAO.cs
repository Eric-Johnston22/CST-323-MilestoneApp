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

        // Method to register a new user asynchronously
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
                        _logger.LogWarningWithContext("Username already exists.");
                        return false;
                    }

                    // Hash the password before saving
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                    _context.Users.Add(user); // Save user to database
                    await _context.SaveChangesAsync();
                    _logger.LogInformationWithContext($"Saving user {user.User_id} to database");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while registering user.");
                    return false;
                }
            }
        }

        // Method to authenticate a user asynchronously
        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            using (_logger.LogMethodEntry(nameof(AuthenticateUserAsync), username, password))
            {
                try
                {
                    // Fetch the user with the given username
                    var user = await _context.Users
                                             .FirstOrDefaultAsync(u => u.Username == username);

                    // Verify the given password with the hashed password stored in the database
                    if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                    {
                        _logger.LogWarningWithContext("Invalid username or password.");
                        return null;
                    }

                    _logger.LogInformationWithContext("User retrieved from database");
                    return user;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while authenticating user.");
                    throw;
                }
            }
        }

        // Method to get a user by ID asynchronously
        public async Task<User> GetUserByIdAsync(int userId)
        {
            using (_logger.LogMethodEntry(nameof(GetUserByIdAsync), userId))
            {
                try
                {
                    // Fetch the user by ID and include related data
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
                        _logger.LogWarningWithContext($"User with ID {userId} not found.");
                    }

                    _logger.LogInformationWithContext($"User {userId} retrieved from database");
                    return user;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching user with ID {userId}.", userId);
                    throw;
                }
            }
        }

        // Method to add a book to the 'Want to Read' list asynchronously
        public async Task<bool> AddToWantToReadAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToWantToReadAsync), userId, bookId))
            {
                if (await IsBookInAnyListAsync(userId, bookId)) 
                {
                    _logger.LogWarningWithContext($"{bookId} is already on one of {userId}'s lists.");
                    return false;
                }
                else
                {
                    var wantToRead = new WantToRead { User_id = userId, Book_id = bookId, DateAdded =  DateTime.Now};
                    _context.WantToRead.Add(wantToRead);
                    _logger.LogInformationWithContext($"Saving book {bookId} to user {userId}'s WantToRead list");
                    await _context.SaveChangesAsync();
                    return true;
                }

            }
        }

        // Method to add a book to the 'Currently Reading' list asynchronously
        public async Task<bool> AddToCurrentlyReadingAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToCurrentlyReadingAsync), userId, bookId))
            {
                if (await IsBookInAnyListAsync(userId, bookId))
                {
                    _logger.LogWarningWithContext($"{bookId} is already on one of {userId}'s lists.");
                    return false;
                }
                else
                {
                    var currentlyReading = new CurrentlyReading { User_id = userId, Book_id = bookId, Start_date = DateTime.Now };
                    _context.CurrentlyReading.Add(currentlyReading);
                    await _context.SaveChangesAsync();
                    _logger.LogInformationWithContext($"Saving book {bookId} to user {userId}'s CurrentlyReading list");
                    return true;
                }
            }
        }

        // Method to add a book to the 'Have Read' list asynchronously
        public async Task<bool> AddToHaveReadAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToHaveReadAsync), userId, bookId))
            {
                if (await IsBookInAnyListAsync(userId, bookId))
                {
                    _logger.LogWarningWithContext($"{bookId} is already on one of {userId}'s lists.");
                    return false;
                }
                else
                {
                    var readingHistory = new ReadingHistory { User_id = userId, Book_id = bookId, Start_date = DateTime.Now, Finish_date = DateTime.Now };
                    _context.ReadingHistory.Add(readingHistory);
                    await _context.SaveChangesAsync();
                    _logger.LogInformationWithContext($"Saving book {bookId} to user {userId}'s 'HaveRead' list");
                    return true;
                }
            }
        }

        // Method to move a book from 'Currently Reading' to 'Have Read' list
        public async Task FinishReadingBookAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(FinishReadingBookAsync), userId, bookId))
            {
                var currentlyReading = await _context.CurrentlyReading.FirstOrDefaultAsync(cr => cr.User_id == userId && cr.Book_id == bookId);
                if (currentlyReading != null)
                {
                    _logger.LogInformationWithContext($"Removing book {bookId} from user {userId}'s 'CurrentlyReading' list");
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
                    _logger.LogInformationWithContext($"Saving book {bookId} to user {userId}'s Reading History");
                }
            }
        }

        // Method to add a review asynchronously
        public async Task AddReviewAsync(Review review)
        {
            using (_logger.LogMethodEntry(nameof(AddReviewAsync), review))
            {
                _logger.LogInformationWithContext($"Attempting to add review: {review.Review_id}");

                // Ensure that the User and Book objects are attached to the context
                _context.Entry(review.User).State = EntityState.Unchanged;
                _context.Entry(review.Book).State = EntityState.Unchanged;

                _context.Reviews.Add(review);

                await _context.SaveChangesAsync();
                _logger.LogInformationWithContext($"Review {review.Review_id} saved to database");
            }
        }

        // Method to check if a book is in any list
        private async Task<bool> IsBookInAnyListAsync(int userId, int bookId)
        {
            return await _context.CurrentlyReading.AnyAsync(cr => cr.User_id == userId && cr.Book_id == bookId) ||
                   await _context.WantToRead.AnyAsync(wtr => wtr.User_id == userId && wtr.Book_id == bookId) ||
                   await _context.ReadingHistory.AnyAsync(hr => hr.User_id == userId && hr.Book_id == bookId);
        }

        public async Task<Review?> GetReviewById(int reviewId)
        {
            using (_logger.LogMethodEntry(nameof(GetReviewById), reviewId))
            {
                _logger.LogInformationWithContext($"Retrieving review {reviewId} from database");
                var review = await _context.Reviews
                    .Include(r => r.Book)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Review_id == reviewId);

                return review;
            }
        }

        public async Task<bool> MoveToCurrentlyReadingAsync(int userId, int bookId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Remove the book from WantToRead list if it exists
                    var wantToReadEntry = await _context.WantToRead
                        .FirstOrDefaultAsync(w => w.User_id == userId && w.Book_id == bookId);

                    if (wantToReadEntry != null)
                    {
                        _context.WantToRead.Remove(wantToReadEntry);
                    }

                    // Check if the book is already in CurrentlyReading or ReadingHistory lists
                    if (await _context.CurrentlyReading.AnyAsync(c => c.User_id == userId && c.Book_id == bookId) ||
                        await _context.ReadingHistory.AnyAsync(r => r.User_id == userId && r.Book_id == bookId))
                    {
                        return false; // Book is already in Currently Reading or Have Read lists
                    }

                    // Add the book to CurrentlyReading list
                    var currentlyReadingEntry = new CurrentlyReading { User_id = userId, Book_id = bookId, Start_date = DateTime.Now };
                    _context.CurrentlyReading.Add(currentlyReadingEntry);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }


        public async Task<List<RecentInteraction>> GetRecentInteractionsAsync()
        {
            using (_logger.LogMethodEntry())
            {
                _logger.LogInformationWithContext("Retrieving the last 10 User interactions");
                var recentInteraction = await _context.WantToRead
                .Select(w => new RecentInteraction
                {
                    UserName = w.User.Username,
                    UserId = w.User_id,
                    InteractionType = "Added to Want to Read",
                    BookTitle = w.Book.Title,
                    BookId = w.Book_id,
                    Date = w.DateAdded
                })
                .Union(_context.CurrentlyReading
                    .Select(c => new RecentInteraction
                    {
                        UserName = c.User.Username,
                        UserId = c.User_id,
                        InteractionType = "Added to Currently Reading",
                        BookTitle = c.Book.Title,
                        BookId = c.Book_id,
                        Date = c.Start_date
                    }))
                .Union(_context.ReadingHistory
                    .Select(h => new RecentInteraction
                    {
                        UserName = h.User.Username,
                        UserId = h.User_id,
                        InteractionType = "Added to Have Read",
                        BookTitle = h.Book.Title,
                        BookId= h.Book_id,
                        Date = h.Finish_date
                    }))
                .Union(_context.Reviews
                    .Select(r => new RecentInteraction
                    {
                        UserName = r.User.Username,
                        UserId = r.User_id,
                        InteractionType = "Reviewed",
                        BookTitle = r.Book.Title,
                        BookId = r.Book_id,
                        Date = r.Review_date
                    }))
                .OrderByDescending(ri => ri.Date)
                .Take(10)
                .ToListAsync();

                return recentInteraction;
            }
        }
    }
}
