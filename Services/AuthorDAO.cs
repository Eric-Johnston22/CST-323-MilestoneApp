using CST_323_MilestoneApp.Controllers;
using CST_323_MilestoneApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CST_323_MilestoneApp.Services
{
    public class AuthorDAO
    {
        private readonly LibraryContext _context;

        public AuthorDAO(LibraryContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<Author> GetAuthorById(int authorId)
        {
            var author = await _context.Authors
                                           .Include(a => a.Books)
                                           .FirstOrDefaultAsync(a => a.Author_Id == authorId);
            return author;
        }
    }
}
