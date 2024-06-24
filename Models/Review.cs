using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CST_323_MilestoneApp.Models
{
    public class Review
    {
        [Key]
        public int Review_id { get; set; }

        [ForeignKey("User_id")]
        public int User_id { get; set; }

        [ForeignKey("Book_id")]
        public int Book_id { get; set; }

        public int Rating { get; set; }

        public string Review_text { get; set; }

        public DateTime Review_date { get; set; }

        public virtual Book Book { get; set; }
        public virtual User User { get; set; }
    }
}
