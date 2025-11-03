// LostAndFoundWebUi/Services/LostAndFoundApiService.cs

using LostAndFoundWebUi.Models;
using Microsoft.AspNetCore.Identity.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
// Assuming your models (DTOs) are accessible or copied here
//using LostAndFoundWebUi.Models;

namespace LostAndFoundWebUi.Services
{
    public class LostAndFoundApiService
    {
        private readonly HttpClient _httpClient;

        // Ctor
        public LostAndFoundApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Abstraction for Login Request (Model sent to API)
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        // Abstraction for Login Response (Response received from API)
        public class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
            // Include any other relevant data from your API response here
            public string Role { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
        }

        /// <summary>
        /// Authenticates the user against the API and returns the JWT token and user info.
        /// </summary>
        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var request = new LoginRequest { Email = email, Password = password };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Assuming the login endpoint is at: /api/Auth/login
                var response = await _httpClient.PostAsync("Auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    // Deserialize the response object from the API
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return loginResponse;
                }

                // If login failed (e.g., 401 Unauthorized/Bad Request)
                return null;
            }
            catch (Exception ex)
            {
                // Log and handle network/connection errors gracefully
                Console.WriteLine($"Error during API login: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ItemDto>> GetLostItemsAsync()
        {
            // Use GetFromJsonAsync to fetch data and automatically deserialize the JSON 
            // into a List of LostItemDto objects.
            var items = await _httpClient.GetFromJsonAsync<List<ItemDto>>("item/get-lost-items");

            // Return the list (it might be null if the API call failed or returned no data)
            return items ?? new List<ItemDto>();
        }
    }
}