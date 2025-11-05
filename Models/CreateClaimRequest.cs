namespace LostAndFoundWebUi.Models
{
    public class CreateClaimRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
