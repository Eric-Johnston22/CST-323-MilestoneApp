using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CST_323_MilestoneApp.Models
{
    public class Review
    {
        [Key]
        [Column("review_id")]
        public int Review_id { get; set; }

        [Column("user_id")]
        public int User_id { get; set; }

        [ForeignKey("User_id")]
        public virtual User User { get; set; }

        [Column("book_id")]
        public int Book_id { get; set; }

        [ForeignKey("Book_id")]
        public virtual Book Book { get; set; }

        [Column("rating")]
        public int Rating { get; set; }

        [Column("review_text")]
        public string Review_text { get; set; }

        [Column("review_date")]
        public DateTime Review_date { get; set; }

    }
}
