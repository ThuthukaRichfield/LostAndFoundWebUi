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

        // 🌟 FIX: New calculated property for Found Items with NO claim
        public List<ItemDto> UnclaimedFoundItems => AllItems
            // Status 1 = Found
            // ClaimedBy == null OR string.IsNullOrEmpty(item.ClaimedBy) = Not claimed
            .Where(item => item.Status == 1 && string.IsNullOrEmpty(item.ClaimedBy))
            .ToList();

        // Property for Found Items WITH a claim (Pending Review)
        public List<ItemDto> PendingClaims => AllItems
            // Status 1 = Found
            // ClaimedBy != null and !string.IsNullOrEmpty(item.ClaimedBy) = Claim submitted
            .Where(item => item.Status == 1 && !string.IsNullOrEmpty(item.ClaimedBy))
            .ToList();

        // Property for Lost Items
        public List<ItemDto> LostItems => AllItems
            .Where(item => item.Status == 0)
            .ToList();

        // Property for Resolved (Claimed) Items
        public List<ItemDto> ClaimedItems => AllItems
            .Where(item => item.Status == 2)
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