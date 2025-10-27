// LostAndFoundWebUi/Services/LostAndFoundApiService.cs

using LostAndFoundWebUi.Models;
using System.Net.Http.Json;
using System.Threading.Tasks;
// Assuming your models (DTOs) are accessible or copied here
//using LostAndFoundWebUi.Models;

namespace LostAndFoundWebUi.Services
{
    public class LostAndFoundApiService
    {
        private readonly HttpClient _httpClient;

        // HttpClient is injected by the framework (HttpClientFactory)
        public LostAndFoundApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //// Example method to send data (POST request) to your API
        //public async Task<bool> CreateReportAsync(ReportModel report)
        //{
        //    // 1. Specify the API endpoint (e.g., /api/reports)
        //    // 2. PostAsJsonAsync serializes the C# object to JSON and sends it.
        //    var response = await _httpClient.PostAsJsonAsync("api/Reports", report);

        //    // Check if the API call was successful (2xx status code)
        //    return response.IsSuccessStatusCode;
        //}

        // You would add more methods here for other CRUD operations (Get, Put, Delete)
        // public async Task<List<Item>> GetLostItemsAsync() { ... }

        public async Task<List<ItemDto>> GetLostItemsAsync()
        {
            // Use GetFromJsonAsync to fetch data and automatically deserialize the JSON 
            // into a List of LostItemDto objects.
            var items = await _httpClient.GetFromJsonAsync<List<ItemDto>>("get-lost-items");

            // Return the list (it might be null if the API call failed or returned no data)
            return items ?? new List<ItemDto>();
        }
    }
}