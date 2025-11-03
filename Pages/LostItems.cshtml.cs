using LostAndFoundWebUi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LostAndFoundWebUi.Models; // Add this using statement

namespace LostAndFoundWebUi.Pages
{
    public class LostItemsModel : PageModel
    {
        private readonly LostAndFoundApiService _apiService;

        // 1. Property to hold the data fetched from the API
        public List<ItemDto> LostItems { get; set; } = new List<ItemDto>();

        public LostItemsModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

        // 2. Change to OnGetAsync and make it call the API service
        public async Task OnGetAsync()
        {
            // Call the service to fetch the data
            LostItems = await _apiService.GetLostItemsAsync();
        }
    }
}