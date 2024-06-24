using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CST_323_MilestoneApp.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int User_id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("username")]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        [Column("password")]
        public string Password { get; set; }

        [Required]
        [StringLength(255)]
        [Column("email")]
        public string Email { get; set; }

        [NotMapped] // Prevent EF from mapping this property to a database column
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [NotMapped]
        public virtual ICollection<WantToRead> WantToRead { get; set; }
        [NotMapped]
        public virtual ICollection<ReadingHistory> ReadingHistory { get; set; }
        [NotMapped]
        public virtual ICollection<Review> Review { get; set; }
        [NotMapped]
        public virtual ICollection<CurrentlyReading> CurrentlyReading { get; set; }

    }
}
