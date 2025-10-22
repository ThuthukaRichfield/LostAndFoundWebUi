using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebUi.Pages
{
    public class DashboardModel : PageModel
    {
        public string Username { get; set; }

        public IActionResult OnGet()
        {
            // Check if user is logged in
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Index"); // Redirect to login
            }
            
            Username = username;
            return Page();
        }
    }
}