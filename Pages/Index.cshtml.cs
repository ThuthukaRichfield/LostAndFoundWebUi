using LostAndFoundWebUi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebUi.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly LostAndFoundApiService _apiService;

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public IndexModel(ILogger<IndexModel> logger, LostAndFoundApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public void OnGet()
        {
            // Clear any old authentication information
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("DisplayName");
            HttpContext.Session.Remove("IsAuthenticated");
            HttpContext.Session.Remove("UserRole");
            HttpContext.Session.Remove("JwtToken");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _logger.LogInformation($"Login attempt to API - Email: {Email}");

            // Call the API Login Endpoint
            var loginResponse = await _apiService.LoginAsync(Email, Password);

            if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
            {
                _logger.LogInformation("API Login successful - storing session variables");

                // Store the JWT Token and other user data from the API response
                HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                HttpContext.Session.SetString("Username", Email);
                HttpContext.Session.SetString("DisplayName", loginResponse.DisplayName);
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("UserRole", loginResponse.Role);

                // 3. Redirect based on role
                if (loginResponse.Role == "Admin")
                {
                    return RedirectToPage("/AdminDashboard");
                }
                else
                {
                    return RedirectToPage("/Dashboard");
                }
            }
            else
            { 
                ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
                return Page();
            }
        }
    }
}
