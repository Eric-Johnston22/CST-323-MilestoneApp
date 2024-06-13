using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CST_323_MilestoneApp.Models
{
    public class Book
    {
        [Key]
        public int book_id { get; set; }

        [Required]
        [StringLength(255)]
        public string title { get; set; }

        [ForeignKey("author_Id")]
        public int author_id { get; set; }

        [Required]
        [StringLength(255)]
        public string isbn { get; set; }

        public DateTime published_date { get; set; }

        [StringLength(255)]
        public string genre { get; set; }

        public virtual Author Author { get; set; }
    }
}
