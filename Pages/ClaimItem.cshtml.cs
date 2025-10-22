using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebUi.Pages
{
    public class ClaimItemModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string ItemId { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string ItemName { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string ItemType { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string Location { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public DateTime Date { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public string ClaimReason { get; set; } = string.Empty;

        [BindProperty]
        public string ContactInfo { get; set; } = string.Empty;

        [BindProperty]
        public string AdditionalInfo { get; set; } = string.Empty;

        [BindProperty]
        public List<IFormFile>? SupportingImages { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Save claim to database
            // Process supporting images

            TempData["SuccessMessage"] = "Claim submitted successfully! The admin will review your claim.";
            return RedirectToPage("/Dashboard");
        }
    }
}