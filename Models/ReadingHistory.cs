using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CST_323_MilestoneApp.Models
{
    public class ReadingHistory
    {
        [Key]
        [Column("reading_history_id")]
        public int ReadingHistory_id { get; set; }

        [Column("user_id")]
        public int User_id { get; set; }

        [ForeignKey("User_id")]
        public virtual User User { get; set; }

        [Column("book_id")]
        public int Book_id { get; set; }

        [ForeignKey("Book_id")]
        public virtual Book Book { get; set; }

        [Column("start_date")]
        public DateTime Start_date { get; set; }
        [Column("finish_date")]
        public DateTime Finish_date { get; set; }

    }
}
