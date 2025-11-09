// LostAndFoundWebUi/Services/LostAndFoundApiService.cs

using LostAndFoundWebUi.Models;
using Microsoft.AspNetCore.Identity.Data;
using System.Buffers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OperationStatus = LostAndFoundWebUi.Models.OperationStatus;
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
            public string Role { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
        }

        // New model for registration request data (to be sent to the API)
        public class ApiUserDetail
        {
            // Note: The API response shows 'role' as an integer (0 for Admin, 1 for User).
            public int Role { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        // 🌟 NEW MODEL: Represents the structure holding the user details
        public class ApiUserWrapper
        {
            public ApiUserDetail? Result { get; set; }
        }

        // 🌟 NEW MODEL: Represents the ReturnObject from the API
        public class ApiReturnObject
        {
            public string Token { get; set; } = string.Empty;
            public ApiUserWrapper? User { get; set; }
        }

        // 🌟 NEW MODEL: The top-level response from the API
        public class ApiLoginResponse
        {
            public bool Status { get; set; }
            public ApiReturnObject? ReturnObject { get; set; }
        }

        public class RegisterRequest1
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            // Defaulting to "User" role for web UI registration
            public int UserRole { get; set; } = 1;
        }

        //public class ItemDto
        //{
        //    public int ItemId { get; set; }
        //    public string Title { get; set; } = string.Empty;
        //    public string Category { get; set; } = string.Empty;
        //    public DateTime CreatedDate { get; set; }
        //    public int Status { get; set; } // Map to ItemStatus enum value (0: Lost, 1: Found, 2: Claimed)
        //                                    // Add other properties as needed
        //}

        public async Task<LoginResponse?> RegisterAsync(RegisterRequest1 registrationData)
        {
            // **Step 1: Construct the query string from the registration data.**
            var queryParams = new Dictionary<string, string>
            {
                { "Email", registrationData.Email },
                { "Password", registrationData.Password },
                { "Name", registrationData.Name },
                { "UserRole", registrationData.UserRole.ToString() }
            };

            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;

            // **Step 2: Construct the full URL with the query string.**
            // Assuming the API registration endpoint is "User/create-user"
            var requestUri = $"User/create-user?{queryString}";

            // **Step 3: Send the POST request with an empty body (since data is in the URL).**
            // Note: Sending sensitive data (like password) in a URL is not recommended 
            // and should typically be done via a POST body.
            var response = await _httpClient.PostAsync(requestUri, new StringContent(string.Empty));

            if (response.IsSuccessStatusCode)
            {
                // The registration endpoint may return a LoginResponse (token, display name, etc.)
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<LoginResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // If the API returns a status code like 400 (Bad Request), return null.
            return null;
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
                var response = await _httpClient.PostAsync("Auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // 🌟 Use the new top-level API response model
                    var apiResponse = JsonSerializer.Deserialize<ApiLoginResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var token = apiResponse?.ReturnObject?.Token;
                    var userDetails = apiResponse?.ReturnObject?.User?.Result;

                    if (!string.IsNullOrEmpty(token) && userDetails != null)
                    {
                        // Map the nested API data to the simple LoginResponse model used by the UI
                        return new LoginResponse
                        {
                            Token = token,
                            // Map the Role integer (0/1) to a readable string ("Admin"/"User")
                            Role = userDetails.Role == 0 ? "Admin" : "User",
                            DisplayName = userDetails.Name // Use Name as DisplayName
                        };
                    }
                    // If API succeeded but response structure was missing token/user details
                    return null;
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

        public async Task<List<ItemDto>> GetItemsAsync(int? status = null, string? searchTerm = null)
        {
            var queryParams = new List<string>();

            if (status.HasValue)
            {
                // The API query uses 'Status' parameter
                queryParams.Add($"Status={status.Value}");
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // The API query uses 'SearchTerm' parameter
                queryParams.Add($"SearchTerm={Uri.EscapeDataString(searchTerm)}");
            }

            var queryString = string.Join("&", queryParams);
            var requestUri = $"Item/get-lost-items{(queryString.Length > 0 ? $"?{queryString}" : string.Empty)}";

            // Assuming your API GetLostItems endpoint now supports the common GetItemsQuery
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<ItemDto>>(requestUri);
                return items ?? new List<ItemDto>();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error fetching items: {ex.Message}");
                return new List<ItemDto>();
            }
        }

        public async Task<OperationStatus> ReportLostItemAsync(ReportLostItemRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Item/report-lost-item", request);

                if (response.IsSuccessStatusCode)
                {
                    // Assuming OperationStatus is defined in your UI project
                    var status = await response.Content.ReadFromJsonAsync<Models.OperationStatus>();
                    return status ?? new OperationStatus();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return OperationStatus.CreateFromException($"API Error: {response.StatusCode}", new Exception(error));
                }
            }
            catch (Exception ex)
            {
                // Handle network or serialization errors
                return OperationStatus.CreateFromException("Network or serialization error occurred.", ex);
            }
        }

        public async Task<OperationStatus> ReportFoundItemAsync(ReportLostItemRequest request)
        {
            try
            {
                //var response = await _httpClient.PostAsJsonAsync("Item/report-found-item", request);
                var response = await _httpClient.PostAsJsonAsync("api/Item/report-found-item", request);

                if (response.IsSuccessStatusCode)
                {
                    // Assuming OperationStatus is defined in your UI project
                    var status = await response.Content.ReadFromJsonAsync<Models.OperationStatus>();
                    return status ?? new OperationStatus();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return OperationStatus.CreateFromException($"API Error: {response.StatusCode}", new Exception(error));
                }
            }
            catch (Exception ex)
            {
                // Handle network or serialization errors
                return OperationStatus.CreateFromException("Network or serialization error occurred.", ex);
            }
        }

        public async Task<OperationStatus> CreateClaimAsync(CreateClaimRequest request)
        {
            try
            {
                // 1. Build the query parameters string
                var queryParams = new Dictionary<string, string>
                {
                    { "UserEmail", request.UserEmail },
                    { "ItemId", request.ItemId.ToString() },
                    { "Reason", request.Reason }
                };

                // This correctly URL-encodes the parameters
                var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;

                // 2. Construct the full request URI with the query string
                // Base route: api/Claim/create-claim
                var requestUri = $"api/Claim/create-claim?{queryString}";

                // 3. Send the POST request with the data in the URL and an empty body.
                // PostAsync is used instead of PostAsJsonAsync since we are sending data via the URL.
                var response = await _httpClient.PostAsync(requestUri, new StringContent(string.Empty));

                if (response.IsSuccessStatusCode)
                {
                    var status = await response.Content.ReadFromJsonAsync<Models.OperationStatus>();
                    return status ?? new OperationStatus();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return OperationStatus.CreateFromException($"API Error: {response.StatusCode}", new Exception(error));
                }
            }
            catch (Exception ex)
            {
                return OperationStatus.CreateFromException("Network or serialization error occurred during claim creation.", ex);
            }
        }
    }
}