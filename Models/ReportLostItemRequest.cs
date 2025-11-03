namespace LostAndFoundWebUi.Models
{
    public class ReportLostItemRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        // public byte[]? Image { get; set; } // Omitted for now as per API command comments
    }
}
