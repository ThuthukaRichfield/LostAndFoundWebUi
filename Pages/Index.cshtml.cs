using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebUi.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        // Test users database
        private readonly Dictionary<string, (string Password, string Name, string Role)> _testUsers = new()
        {
            // Admin users
            {"admin@richfield.co.za", ("admin", "System Administrator", "Admin")},
            
            // Student users
            {"student1@richfield.co.za", ("student123", "Alice Johnson", "Student")},
            {"student2@richfield.co.za", ("student123", "Bob Smith", "Student")},
            {"john.doe@richfield.co.za", ("student123", "John Doe", "Student")},
        };

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            _logger.LogInformation($"Login attempt - Email: {Email}, Password: {Password}");

            // Check if user exists
            if (_testUsers.TryGetValue(Email, out var user))
            {
                _logger.LogInformation($"User found: {user.Name}, Role: {user.Role}");
                
                // Check password
                if (user.Password == Password)
                {
                    _logger.LogInformation("Password correct - setting session variables");
                    
                    // Store user information in session
                    HttpContext.Session.SetString("Username", Email);
                    HttpContext.Session.SetString("DisplayName", user.Name);
                    HttpContext.Session.SetString("IsAuthenticated", "true");
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // Log session values for debugging
                    _logger.LogInformation($"Session set - Username: {Email}, Role: {user.Role}, DisplayName: {user.Name}");

                    // Redirect based on role
                    if (user.Role == "Admin")
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
                    _logger.LogWarning("Password incorrect");
                    ModelState.AddModelError(string.Empty, "Invalid password.");
                }
            }
            else
            {
                _logger.LogWarning("User not found in test users");
                ModelState.AddModelError(string.Empty, "User not found.");
            }

            return Page();
        }
    }
}