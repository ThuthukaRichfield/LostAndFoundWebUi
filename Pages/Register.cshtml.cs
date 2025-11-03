using LostAndFoundWebUi.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static LostAndFoundWebUi.Services.LostAndFoundApiService;

namespace LostAndFoundWebUi.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly LostAndFoundApiService _apiService;

        // Bind properties to the form inputs
        [BindProperty]
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public RegisterModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

        public void OnGet()
        {
            // Optional: Clear session or check for existing authentication here
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var request = new RegisterRequest1
            {
                Email = Email,
                Password = Password,
                Name = FullName,
                // The UserRole property defaults to "User" in the RegisterRequest model
            };

            // Call the API Registration Endpoint
            var loginResponse = await _apiService.RegisterAsync(request);

            if (loginResponse != null)
                //if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
            {
                // SUCCESS: Registration succeeded, automatically log the user in via session
                HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                HttpContext.Session.SetString("Username", Email);
                HttpContext.Session.SetString("DisplayName", loginResponse.DisplayName);
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("UserRole", loginResponse.Role);

                // Redirect to the main Dashboard after successful registration/login
                return RedirectToPage("/Dashboard");
            }
            else
            {
                // FAILURE: Display error message
                ModelState.AddModelError(string.Empty, "Registration failed. This email may already be registered, or there was a server error.");
                return Page();
            }
        }
    }
}
