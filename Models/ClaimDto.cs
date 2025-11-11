namespace LostAndFoundWebUi.Models
{
    public class ClaimDto
    {
        public int ClaimId { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public string CreatedBy { get; set; } = string.Empty; // User email
        public DateTime CreatedDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime DateLost { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
