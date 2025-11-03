using LostAndFoundWebUi.Models;
using LostAndFoundWebUi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace LostAndFoundWebUi.Pages
{
    public class ReportFoundModel : PageModel
    {
        private readonly LostAndFoundApiService _apiService;

        // Ctor: Inject the API Service
        public ReportFoundModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

        // --- View Properties (BindProperty for form) ---

        [BindProperty]
        [Required]
        public string ItemName { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        public string Category { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        public string Location { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateLost { get; set; } = DateTime.Today;

        [BindProperty]
        [Required]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        // Feedback properties
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        // --- Handlers ---

        public void OnGet()
        {
            // Check authentication if needed, otherwise just return Page
        }

        public async Task<IActionResult> OnPostAsync() // Changed to Async
        {
            // 1. Basic Model Validation
            if (!ModelState.IsValid)
            {
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
            var request = new ReportLostItemRequest
            {
                UserEmail = userEmail,
                Title = ItemName,
                Category = Category,
                Location = Location,
                Description = Description,
                // Note: ImageFile is not sent in the current API model. 
                // To support images, you'd need to convert IFormFile to byte[] and update the DTO/API command.
            };

            // 4. Call the API
            var result = await _apiService.ReportFoundItemAsync(request);

            // 5. Handle the API result
            if (result.Status)
            {
                TempData["SuccessMessage"] = "Lost item reported successfully!";
                return RedirectToPage("/Dashboard");
            }
            else
            {
                ErrorMessage = $"Failed to report item: {result.Message}";
                return Page();
            }
        }
    }
}