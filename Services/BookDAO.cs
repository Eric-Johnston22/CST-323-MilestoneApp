using CST_323_MilestoneApp.Controllers;
using CST_323_MilestoneApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CST_323_MilestoneApp.Services
{
    public class BookDAO
    {
        private readonly LibraryContext _context;

        public BookDAO(LibraryContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books.Include(b => b.Author).ToListAsync();
        }

        // Additional methods for CRUD operations to be added here
    }
}
