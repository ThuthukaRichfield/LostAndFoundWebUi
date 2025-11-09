using LostAndFoundWebUi.Models;
using LostAndFoundWebUi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace LostAndFoundWebUi.Pages
{
    public class ClaimItemModel : PageModel
    {
        private readonly LostAndFoundApiService _apiService;

        public ClaimItemModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "Item ID is required.")]
        public int ItemId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ItemName { get; set; } = "Item Details Loading...";

        [BindProperty(SupportsGet = true)]
        public string ItemType { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string Location { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public DateTime Date { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "You must provide a reason for claiming this item.")]
        public string ClaimReason { get; set; } = string.Empty;

        // --- Feedback Properties ---
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            if (ItemId == 0)
            {
                ErrorMessage = "No Item ID provided.";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Basic Model Validation
            if (!ModelState.IsValid)
            {
                // This should now only fail if ItemId or ClaimReason is missing
                return Page();
            }

            // 2. Get the current user's email from session
            var userEmail = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(userEmail))
            {
                ErrorMessage = "User session expired. Please log in again.";
                return Page();
            }

            // 3. Map form data to the API request DTO
            var request = new CreateClaimRequest
            {
                UserEmail = userEmail,
                ItemId = ItemId,
                Reason = ClaimReason
            };

            // 4. Call the API
            var result = await _apiService.CreateClaimAsync(request);

            // 5. Handle the API result
            if (result.Status)
            {
                TempData["SuccessMessage"] = "Claim submitted successfully! The owner will be notified.";
                return RedirectToPage("/Dashboard");
            }
            else
            {
                ErrorMessage = $"Failed to create claim: {result.Message}";
                return Page();
            }
        }
    }
}