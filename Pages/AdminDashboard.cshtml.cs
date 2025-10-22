using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebUi.Pages
{
    public class AdminDashboardModel : PageModel
    {
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            // Check if user is logged in AND is admin
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true" || userRole != "Admin")
            {
                return RedirectToPage("/Index");
            }
            
            Username = HttpContext.Session.GetString("Username") ?? "Admin";
            DisplayName = HttpContext.Session.GetString("DisplayName") ?? "Administrator";
            UserRole = "Admin";
            
            return Page();
        }
    }
}