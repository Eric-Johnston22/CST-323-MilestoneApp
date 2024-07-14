using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CST_323_MilestoneApp.Models
{
    public class WantToRead
    {
        [Key]
        [Column("want_to_read_id")]
        public int WantToRead_id { get; set; }

        [ForeignKey("User_id")]
        public int User_id { get; set; }

        [ForeignKey("Book_id")]
        public int Book_id { get; set; }

        [Column("date_added")]
        public DateTime DateAdded { get; set; }

        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
    }
}
