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

        // Constructor to initialize the context and logger
        public AuthorDAO(LibraryContext context, ILogger<AuthorDAO> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Method to get all authors asynchronously
        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            using (_logger.LogMethodEntry(nameof(GetAllAuthorsAsync)))
            {
                // Fetch all authors from the database
                var authors = await _context.Authors.ToListAsync();
                _logger.LogInformationWithContext($"Query returned {authors.Count} authors");
                return authors;
            }
        }

        // Method to get an author by ID asynchronously
        public async Task<Author> GetAuthorById(int authorId)
        {
            using (_logger.LogMethodEntry(nameof(GetAuthorById), authorId))
            {
                // Fetch the author and include their books
                var author = await _context.Authors
                                           .Include(a => a.Books)
                                           .FirstOrDefaultAsync(a => a.Author_id == authorId);

                // Log whether the author was found or not
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

