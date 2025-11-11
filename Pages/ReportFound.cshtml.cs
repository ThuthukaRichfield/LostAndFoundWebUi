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

        public ReportFoundModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

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

        public async Task<IActionResult> OnPostAsync()
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

            var request = new ReportLostItemRequest
            {
                UserEmail = userEmail,
                Title = ItemName,
                Category = Category,
                Location = Location,
                Description = Description,
            };

            var result = await _apiService.ReportFoundItemAsync(request);

            if (result.Status)
            {
                var userRole = HttpContext.Session.GetString("UserRole");

                TempData["SuccessMessage"] = "Lost item reported successfully!";
                if (userRole == "Admin")
                {
                    return RedirectToPage("AdminDashboard");
                }
                else
                {
                    return RedirectToPage("/Dashboard");
                }
            }
            else
            {
                ErrorMessage = $"Failed to report item: {result.Message}";
                return Page();
            }
        }
    }
}