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

        // Belirtilen konuma göre HotelService'ten otel bilgilerini alır
        public async Task<List<Hotel>> GetHotelsByLocation(string location)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    WriteIndented = true
                };


                // HotelService'teki GetByLocation endpoint'ine istek yapıyoruz
                var response = await _httpClient.GetAsync($"https://localhost:5000/api/hotel/bylocation?location={location}");

                // İstek başarısız olursa exception fırlat
                response.EnsureSuccessStatusCode();

                // Gelen cevabı List<Hotel> türüne deserialize et
                var hotels = await response.Content.ReadAsStringAsync();
                var hotelsData = JsonSerializer.Deserialize<List<Hotel>>(hotels, options);

                return hotelsData ?? new List<Hotel>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HotelService'e istek yapılırken hata oluştu: {ex.Message}");
                return new List<Hotel>(); // Hata durumunda boş bir otel listesi döner
            }
        }
    }
}
