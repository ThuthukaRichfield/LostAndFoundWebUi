namespace LostAndFoundWebUi.Models
{
    public class ItemDto
    {
        public int ItemId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }
        //public ItemStatus Status { get; set; }
        public int Status { get; set; } // Map to ItemStatus enum value (0: Lost, 1: Found, 2: Claimed)
        public string? ClaimedBy { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}
