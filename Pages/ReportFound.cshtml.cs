using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebUi.Pages
{
    public class ReportFoundModel : PageModel
    {
        [BindProperty]
        public string ItemName { get; set; } = string.Empty;

        [BindProperty]
        public string Category { get; set; } = string.Empty;

        [BindProperty]
        public string Location { get; set; } = string.Empty;

        [BindProperty]
        public DateTime DateFound { get; set; } = DateTime.Today;

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

            TempData["SuccessMessage"] = "Found item reported successfully!";
            return RedirectToPage("/Dashboard");
        }
    }
}