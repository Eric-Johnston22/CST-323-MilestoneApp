using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CST_323_MilestoneApp.Models
{
    [Table("books")] // specify the table name
    public class Book
    {
        [Key]
        [Column("book_id")]
        public int Book_id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("title")]
        public string Title { get; set; }

        [ForeignKey("Author_id")]
        [Column("author_id")]
        public int Author_id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("isbn")]
        public string ISBN { get; set; }

        [Column("published_date")]
        public DateTime Published_date { get; set; }

        [StringLength(255)]
        [Column("genre")]
        public string Genre { get; set; }

        public virtual Author Author { get; set; }

        // Navigation properties
        public virtual ICollection<Review> Review { get; set; }
        public virtual ICollection<WantToRead> WantToRead { get; set; }
        public virtual ICollection<CurrentlyReading> CurrentlyReading { get; set; }
        public virtual ICollection<ReadingHistory> ReadingHistory { get; set; }
    }
}
