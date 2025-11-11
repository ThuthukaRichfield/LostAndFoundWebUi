using LostAndFoundWebUi.Models;
using LostAndFoundWebUi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebUi.Pages
{
    public class AdminClaimsModel : PageModel
    {
        private readonly LostAndFoundApiService _apiService;

        [BindProperty(SupportsGet = true)]
        public int? ItemId { get; set; } // To receive the item ID from the URL

        // Properties to receive the POST data from the form
        [BindProperty]
        public int ClaimId { get; set; }

        [BindProperty]
        public bool IsApproved { get; set; }
        // ^ The name must match the 'name' attribute on the submit buttons

        public List<ClaimDto> Claims { get; set; } = new List<ClaimDto>();

        public string ItemTitle { get; set; } = "Selected Item";

        public AdminClaimsModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!ItemId.HasValue || ItemId.Value <= 0)
            {
                return Page();
            }

            Claims = await _apiService.GetClaimsByItemAsync(ItemId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostManageClaimAsync()
        {
            if (ClaimId <= 0)
            {
                // Should not happen if the hidden field is set correctly
                TempData["ErrorMessage"] = "Invalid Claim ID received.";
                return RedirectToPage(new { ItemId = ItemId });
            }

            // Call the service method to update the status via the API
            bool success = await _apiService.ManageClaimStatusAsync(ClaimId, IsApproved);

            if (success)
            {
                TempData["SuccessMessage"] = $"Claim #{ClaimId} successfully {(IsApproved ? "approved" : "rejected")}.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to {(IsApproved ? "approve" : "reject")} Claim #{ClaimId}. Please check the API log.";
            }

            // Redirect back to the dashboard
            return RedirectToPage("AdminDashboard");
        }
    }
}