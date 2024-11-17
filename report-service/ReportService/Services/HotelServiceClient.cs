using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using ReportService.Models;

namespace ReportService.Services
{
    public class HotelServiceClient
    {
        private readonly HttpClient _httpClient;

        public HotelServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Retrieves hotel information from HotelService for the specified location
        public async Task<List<Hotel>> GetHotelsByLocation(string location)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    WriteIndented = true
                };


                // Sending a request to the GetByLocation endpoint in HotelService
                var response = await _httpClient.GetAsync($"https://localhost:5000/api/hotel/bylocation?location={location}");

                response.EnsureSuccessStatusCode();

                var hotels = await response.Content.ReadAsStringAsync();
                var hotelsData = JsonSerializer.Deserialize<List<Hotel>>(hotels, options);

                return hotelsData ?? new List<Hotel>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An Error occured while sending request to HotelService: {ex.Message}");
                return new List<Hotel>(); // Returns an empty list of hotels in case of an error

            }
        }
    }
}
