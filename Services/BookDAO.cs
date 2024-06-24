using CST_323_MilestoneApp.Controllers;
using CST_323_MilestoneApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CST_323_MilestoneApp.Services
{
    public class BookDAO
    {
        private readonly LibraryContext _context;
        private readonly ILogger<BookDAO> _logger;

        public BookDAO(LibraryContext context, ILogger<BookDAO> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                return await _context.Books
                                     .Include(b => b.Author)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books.");
                throw;
            }
        }

        // Additional methods for CRUD operations to be added here

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            try
            {
                var book = await _context.Books
                                         .Include(b => b.Author)
                                         .FirstOrDefaultAsync(b => b.Book_id == bookId);
                if (book == null)
                {
                    _logger.LogWarning($"Book with ID {bookId} not found.");
                }
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching book with ID {bookId}.");
                throw;
            }
        }
    }
}
