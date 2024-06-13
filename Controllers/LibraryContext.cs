using CST_323_MilestoneApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CST_323_MilestoneApp.Controllers
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define primary keys
            modelBuilder.Entity<Book>()
                .HasKey(b => b.book_id);

            modelBuilder.Entity<Author>()
                .HasKey(a => a.author_Id);

            // Define foreign key relationship
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.author_id)
                .OnDelete(DeleteBehavior.Cascade);  // Ensure correct delete behavior
        }
    }
}
