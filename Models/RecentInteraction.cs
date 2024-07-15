namespace CST_323_MilestoneApp.Models
{
    public class RecentInteraction
    {
        public required string UserName { get; set; }
        public int UserId { get; set; }
        public required string InteractionType { get; set; } // e.g., "Added to Want to Read", "Reviewed"
        public required string BookTitle { get; set; }
        public int BookId { get; set; }
        public DateTime Date { get; set; }
    }
}
