using CST_323_MilestoneApp.Controllers;
using CST_323_MilestoneApp.Models;
using Microsoft.EntityFrameworkCore;
using CST_323_MilestoneApp.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            using (_logger.LogMethodEntry())
            {

                try
                {
                    var books = await _context.Books.Include(b => b.Author).ToListAsync();
                    _logger.LogInformation("Query returned {count} books", books.Count);
                    return books;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching books.");
                    throw;
                }
            }
        }

        // Additional methods for CRUD operations to be added here

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(GetBookByIdAsync), bookId))
            {
                var book = await _context.Books
                                         .Include(b => b.Author)
                                         .FirstOrDefaultAsync(b => b.Book_id == bookId);
                if (book == null)
                {
                    _logger.LogWarning("Book with id: {Id} not found", bookId);
                }
                else
                {
                    _logger.LogInformation("Query returned book with id: {id}", bookId);
                }
                return book;
            }   
        }
    }
}
