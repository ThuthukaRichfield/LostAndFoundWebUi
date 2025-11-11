using LostAndFoundWebUi.Models;
using LostAndFoundWebUi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebUi.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly LostAndFoundApiService _apiService;

        public AdminDashboardModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

        public string DisplayName { get; set; } = "Admin User";
        public string Username { get; set; } = string.Empty;
        public string UserRole { get; set; } = "Admin";

        public List<ItemDto> AllItems { get; set; } = new List<ItemDto>();

        public List<ItemDto> UnclaimedFoundItems => AllItems
            .Where(item => item.Status == 1 && string.IsNullOrEmpty(item.ClaimedBy))
            .ToList();

        // Found Items WITH a claim (Pending Review)
        public List<ItemDto> PendingClaims => AllItems
            .Where(item => item.Status == 1 && !string.IsNullOrEmpty(item.ClaimedBy))
            .ToList();

        // Lost Items
        public List<ItemDto> LostItems => AllItems
            .Where(item => item.Status == 0)
            .ToList();

        // Resolved (Claimed) Items
        public List<ItemDto> ClaimedItems => AllItems
            .Where(item => item.Status == 2)
            .ToList();


        public async Task OnGetAsync()
        {
            // Set User Details
            Username = HttpContext.Session.GetString("Username") ?? "Unknown";
            DisplayName = HttpContext.Session.GetString("DisplayName") ?? "Administrator";

            var allItems = await _apiService.GetLostItemsAsync();

            if (allItems != null)
            {
                // Sort all items descending by date for the main timeline view
                AllItems = allItems.OrderByDescending(i => i.CreatedDate).ToList();
            }
        }
    }
}