﻿using CST_323_MilestoneApp.Controllers;
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
                        _logger.LogWarningWithContext("Username already exists.");
                        return false;
                    }

                    // Hash the password before saving (optional, not implemented here for simplicity)
                    // user.Password = HashPassword(user.Password);

                    _context.Users.Add(user);
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
                        _logger.LogWarningWithContext("Invalid username or password.");
                        return user;
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

        public async Task AddToWantToReadAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToWantToReadAsync), userId, bookId))
            {
                var wantToRead = new WantToRead { User_id = userId, Book_id = bookId };
                _context.WantToRead.Add(wantToRead);
                await _context.SaveChangesAsync();
                _logger.LogInformationWithContext($"Saving book {bookId} to user {userId}'s WantToRead list");
            }
        }

        public async Task AddToCurrentlyReadingAsync(int userId, int bookId)
        {
            using (_logger.LogMethodEntry(nameof(AddToCurrentlyReadingAsync), userId, bookId))
            {
                var currentlyReading = new CurrentlyReading { User_id = userId, Book_id = bookId, Start_date = DateTime.Now };
                _context.CurrentlyReading.Add(currentlyReading);
                await _context.SaveChangesAsync();
                _logger.LogInformationWithContext($"Saving book {bookId} to user {userId}'s CurrentlyReading list");
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
                _logger.LogInformationWithContext($"Saving book {bookId} to user {userId}'s 'HaveRead' list");
            }
        }

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
    }
}
