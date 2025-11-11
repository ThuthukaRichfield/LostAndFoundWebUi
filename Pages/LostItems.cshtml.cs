using LostAndFoundWebUi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LostAndFoundWebUi.Models; // Add this using statement

namespace LostAndFoundWebUi.Pages
{
    public class LostItemsModel : PageModel
    {
        private readonly LostAndFoundApiService _apiService;

        public List<ItemDto> LostItems { get; set; } = new List<ItemDto>();

        public LostItemsModel(LostAndFoundApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            LostItems = await _apiService.GetLostItemsAsync();
        }
    }
}