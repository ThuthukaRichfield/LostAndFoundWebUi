using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebUi.Pages
{
    public class ReportLostModel : PageModel
    {
        [BindProperty]
        public string ItemName { get; set; } = string.Empty;

        [BindProperty]
        public string Category { get; set; } = string.Empty;

        [BindProperty]
        public string Location { get; set; } = string.Empty;

        [BindProperty]
        public DateTime DateLost { get; set; } = DateTime.Today;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Save to database
            // Process image upload if exists

            TempData["SuccessMessage"] = "Lost item reported successfully!";
            return RedirectToPage("/Dashboard");
        }
    }
}