using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebUi.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Simple login check - using your test credentials
            if (Email == "admin@richfield.co.za" && Password == "admin")
            {
                // Set session or authentication cookie
                HttpContext.Session.SetString("Username", Email);
                return RedirectToPage("/Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Invalid login credentials");
            return Page();
        }
    }
}