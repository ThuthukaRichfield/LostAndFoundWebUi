using LostAndFoundWebUi.Services;
using LostAndFoundWebUi.Models; // Ensure ItemDto is accessible, assuming it's here
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostAndFoundWebUi.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly LostAndFoundApiService _apiService;

        // Properties for the welcome message
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;

        // **NEW PROPERTY** to hold all fetched items
        public List<ItemDto> AllItems { get; set; } = new List<ItemDto>();

        // Constructor for Dependency Injection
        public DashboardModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync() // Changed to async
        {
            // 1. Check if user is logged in (Authentication Check)
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToPage("/Index");
            }

            // 2. Load User Details from Session
            Username = HttpContext.Session.GetString("Username") ?? "User";
            DisplayName = HttpContext.Session.GetString("DisplayName") ?? "User";
            UserRole = HttpContext.Session.GetString("UserRole") ?? "User";

            // 3. **API Call to fetch all items** (Lost, Found, All)
            // Calling the unified GetItemsAsync method with no parameters to get all items.
            AllItems = await _apiService.GetLostItemsAsync();

            return Page();
        }
    }
}