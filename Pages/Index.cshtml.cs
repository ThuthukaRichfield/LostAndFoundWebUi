using LostAndFoundWebUi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging; // Ensure this is imported

namespace LostAndFoundWebUi.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly LostAndFoundApiService _apiService; // Inject the API service

        [BindProperty]
        public string Email { get; set; } = "adminTD4@admin.com";

        [BindProperty]
        public string Password { get; set; } = "Admin@123";

        public IndexModel(ILogger<IndexModel> logger, LostAndFoundApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public void OnGet()
        {
            // Clear any old authentication information on page load (logout on entry)
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("DisplayName");
            HttpContext.Session.Remove("IsAuthenticated");
            HttpContext.Session.Remove("UserRole");
            // 🌟 Crucially, remove the JWT token
            HttpContext.Session.Remove("JwtToken");
        }

        public async Task<IActionResult> OnPostAsync() // Changed to async Task<IActionResult>
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _logger.LogInformation($"Login attempt to API - Email: {Email}");

            // 1. Call the API Login Endpoint
            var loginResponse = await _apiService.LoginAsync(Email, Password);

            if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
            {
                _logger.LogInformation("API Login successful - storing session variables");

                // 2. SUCCESS: Store the JWT Token and other user data from the API response
                HttpContext.Session.SetString("JwtToken", loginResponse.Token);

                // Assuming the API sends Role and DisplayName in the LoginResponse:
                // If not, you may need a separate API call to get user details
                HttpContext.Session.SetString("Username", Email);
                HttpContext.Session.SetString("DisplayName", loginResponse.DisplayName);
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("UserRole", loginResponse.Role);

                _logger.LogInformation($"User Role: {loginResponse.Role}");

                // 3. Redirect based on role
                if (loginResponse.Role == "Admin")
                {
                    _logger.LogInformation("Redirecting to AdminDashboard");
                    return RedirectToPage("/AdminDashboard");
                }
                else
                {
                    _logger.LogInformation("Redirecting to regular Dashboard");
                    return RedirectToPage("/Dashboard");
                }
            }
            else
            {
                // 4. FAILURE: Show error message
                _logger.LogWarning("API Login failed or token was empty.");
                ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
                return Page();
            }
        }
    }
}
