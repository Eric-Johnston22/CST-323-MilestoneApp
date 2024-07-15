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

        // Method to get all books asynchronously
        public async Task<List<Book>> GetAllBooksAsync()
        {
            using (_logger.LogMethodEntry())
            {

                try
                {
                    // Fetch all books from the database and include their authors
                    var books = await _context.Books.Include(b => b.Author).ToListAsync();
                    _logger.LogInformationWithContext($"Query returned {books.Count} books");
                    return books;
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur during the fetch operation
                    _logger.LogError(ex, "Error occurred while fetching books.");
                    throw;
                }
            }
        }

        // Method to get a book by ID asynchronously
        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            using (_logger.LogMethodEntry(nameof(GetBookByIdAsync), bookId))
            {
                // Fetch the book by ID and include its author
                var book = await _context.Books
                                         .Include(b => b.Author)
                                         .FirstOrDefaultAsync(b => b.Book_id == bookId);
                if (book == null)
                {
                    _logger.LogWarningWithContext($"Book with id: {bookId} not found"); // Log a warning if the book is not found
                }
                else
                {
                    _logger.LogInformationWithContext($"Query returned book with id: {bookId}"); // Log information if the book is found
                }
                return book;
            }   
        }
    }
}
