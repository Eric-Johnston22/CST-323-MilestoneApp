using CST_323_MilestoneApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CST_323_MilestoneApp.Controllers
{
    public class LibraryContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory
           = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CurrentlyReading> CurrentlyReading { get; set; }
        public DbSet<WantToRead> WantToRead { get; set; }
        public DbSet<ReadingHistory> ReadingHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLoggerFactory(MyLoggerFactory); // Use the logger factory
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define table names
            modelBuilder.Entity<Book>().ToTable("books");
            modelBuilder.Entity<Author>().ToTable("authors");
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Review>().ToTable("reviews");
            modelBuilder.Entity<CurrentlyReading>().ToTable("currentlyreading");
            modelBuilder.Entity<ReadingHistory>().ToTable("readinghistory");
            modelBuilder.Entity<WantToRead>().ToTable("wanttoread");

            // Define primary keys
            modelBuilder.Entity<User>().HasKey(u => u.User_id);
            modelBuilder.Entity<Book>().HasKey(b => b.Book_id);
            modelBuilder.Entity<Review>().HasKey(r => r.Review_id);
            modelBuilder.Entity<Author>().HasKey(a => a.Author_id);
            modelBuilder.Entity<WantToRead>().HasKey(w => w.WantToRead_id);
            modelBuilder.Entity<CurrentlyReading>().HasKey(c => c.CurrentlyReading_id);
            modelBuilder.Entity<ReadingHistory>().HasKey(rh => rh.ReadingHistory_id);

            // Define foreign key relationships
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.Author_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Review)
                .HasForeignKey(r => r.Book_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Review)
                .HasForeignKey(r => r.User_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WantToRead>()
                .HasOne(w => w.User)
                .WithMany(u => u.WantToRead)
                .HasForeignKey(w => w.User_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WantToRead>()
                .HasOne(w => w.Book)
                .WithMany(b => b.WantToRead)
                .HasForeignKey(w => w.Book_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CurrentlyReading>()
                .HasOne(c => c.User)
                .WithMany(u => u.CurrentlyReading)
                .HasForeignKey(c => c.User_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CurrentlyReading>()
                .HasOne(c => c.Book)
                .WithMany(b => b.CurrentlyReading)
                .HasForeignKey(c => c.Book_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReadingHistory>()
                .HasOne(rh => rh.User)
                .WithMany(u => u.ReadingHistory)
                .HasForeignKey(rh => rh.User_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReadingHistory>()
                .HasOne(rh => rh.Book)
                .WithMany(b => b.ReadingHistory)
                .HasForeignKey(rh => rh.Book_id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
