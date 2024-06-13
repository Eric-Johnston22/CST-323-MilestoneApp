using System.ComponentModel.DataAnnotations;

namespace CST_323_MilestoneApp.Models
{
    public class Author
    {
        [Key]
        public int author_Id { get; set; }

        [Required]
        [StringLength(255)]
        public string name { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
