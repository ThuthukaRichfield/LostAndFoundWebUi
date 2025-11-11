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

        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;

        public List<ItemDto> AllItems { get; set; } = new List<ItemDto>();

        public DashboardModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToPage("/Index");
            }

            Username = HttpContext.Session.GetString("Username") ?? "User";
            DisplayName = HttpContext.Session.GetString("DisplayName") ?? "User";
            UserRole = HttpContext.Session.GetString("UserRole") ?? "User";

            AllItems = await _apiService.GetLostItemsAsync();

            return Page();
        }
    }
}