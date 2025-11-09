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

        // 🌟 PROPERTY TO HOLD ALL ITEMS (full list)
        public List<ItemDto> AllItems { get; set; } = new List<ItemDto>();

        // 🌟 NEW: Property to hold Pending Claims subset for clarity
        // A pending claim is an Item that is Found (Status=1) AND has a ClaimedBy user.
        public List<ItemDto> PendingClaims => AllItems
            .Where(item => item.Status == 1 && !string.IsNullOrEmpty(item.ClaimedBy))
            .ToList();


        public async Task OnGetAsync()
        {
            // Set User Details
            Username = HttpContext.Session.GetString("Username") ?? "Unknown";
            DisplayName = HttpContext.Session.GetString("DisplayName") ?? "Administrator";

            // Assuming _apiService.GetAllItemsAsync() is implemented and returns List<ItemDto>
            var allItems = await _apiService.GetLostItemsAsync();

            if (allItems != null)
            {
                // Sort all items descending by date for the main timeline view
                AllItems = allItems.OrderByDescending(i => i.CreatedDate).ToList();
            }
        }
    }
}