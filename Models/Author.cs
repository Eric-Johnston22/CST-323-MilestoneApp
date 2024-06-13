using System.ComponentModel.DataAnnotations;

namespace CST_323_MilestoneApp.Models
{
    public class Author
    {
        [Key]
        public int Author_Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<Book>? Books { get; set; }
    }
}
