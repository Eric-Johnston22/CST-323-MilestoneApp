using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CST_323_MilestoneApp.Models
{
    [Table("authors")]
    public class Author
    {
        [Key]
        [Column("author_id")]
        public int Author_id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("name")]
        public string Name { get; set; }

        public virtual ICollection<Book>? Books { get; set; }
    }
}
