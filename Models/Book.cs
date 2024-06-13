using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CST_323_MilestoneApp.Models
{
    public class Book
    {
        [Key]
        public int Book_id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [ForeignKey("Author_Id")]
        public int Author_id { get; set; }

        [Required]
        [StringLength(255)]
        public string ISBN { get; set; }

        public DateTime Published_date { get; set; }

        [StringLength(255)]
        public string Genre { get; set; }

        public virtual Author Author { get; set; }
    }
}
