namespace LostAndFoundWebUi.Models
{
    public class ReportLostItemRequest
    {
        public string UserEmail { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        // public byte[]? Image { get; set; } // Omitted for now as per API command comments
    }
}
