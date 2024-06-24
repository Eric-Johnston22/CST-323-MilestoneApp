﻿using CST_323_MilestoneApp.Controllers;
using CST_323_MilestoneApp.Models;
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
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user.");
                return false;
            }
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                                         .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
                if (user == null)
                {
                    _logger.LogWarning("Invalid username or password.");
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while authenticating user.");
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
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
                    _logger.LogWarning($"User with ID {userId} not found.");
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching user with ID {userId}.");
                throw;
            }
        }
    }
}
