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

        public List<ClaimDto> Claims { get; set; } = new List<ClaimDto>();

        // You'll need the Item's title/info to display at the top of the page
        // You'd need another API call to get this or pass it via route/session
        public string ItemTitle { get; set; } = "Selected Item";

        public AdminClaimsModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // If ItemId is null, perhaps redirect or show a list of items to select from
            if (!ItemId.HasValue || ItemId.Value <= 0)
            {
                // This assumes you have a page to view all items, e.g., AdminItems
                // For now, let's stop and show an error or empty list.
                return Page();
            }

            // 1. Fetch claims for the specific ItemId
            Claims = await _apiService.GetClaimsByItemAsync(ItemId.Value);

            // 2. (Optional, but recommended) Fetch item details if needed for the title/header
            // ItemTitle = await _apiService.GetItemTitleAsync(ItemId.Value); 

            return Page();
        }

        // ... (Add OnPost methods for Approve/Reject here later) ...
    }
}