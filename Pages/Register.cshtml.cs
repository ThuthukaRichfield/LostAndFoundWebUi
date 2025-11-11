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
            };

            var loginResponse = await _apiService.RegisterAsync(request);

            if (loginResponse != null)
            {
                HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                HttpContext.Session.SetString("Username", Email);
                HttpContext.Session.SetString("DisplayName", loginResponse.DisplayName);
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("UserRole", loginResponse.Role);

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
