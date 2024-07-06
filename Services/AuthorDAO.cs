using CST_323_MilestoneApp.Controllers;
using CST_323_MilestoneApp.Models;
using CST_323_MilestoneApp.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CST_323_MilestoneApp.Services
{
    public class AuthorDAO
    {
        private readonly LibraryContext _context;
        private readonly ILogger<AuthorDAO> _logger;
        

        public AuthorDAO(LibraryContext context, ILogger<AuthorDAO> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            using (_logger.LogMethodEntry(nameof(GetAllAuthorsAsync)))
            {
                {
                    var authors = await _context.Authors.ToListAsync();
                    _logger.LogInformationWithContext($"Query returned {authors.Count} authors");
                    return authors;
                }
            }
        }

        public async Task<Author> GetAuthorById(int authorId)
        {
            using (_logger.LogMethodEntry(nameof(GetAuthorById), authorId))
            {
                var author = await _context.Authors
                                               .Include(a => a.Books)
                                               .FirstOrDefaultAsync(a => a.Author_id == authorId);

                if (author != null)
                {
                    _logger.LogInformationWithContext($"Query returned author with id: {authorId}");
                }
                else
                {
                    _logger.LogWarningWithContext($"Author with id: {authorId} not found");
                }
                return author;

            }
        }
    }
}
