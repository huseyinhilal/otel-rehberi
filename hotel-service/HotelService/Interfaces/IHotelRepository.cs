using System.Collections.Generic;
using System.Threading.Tasks;
using HotelService.Models;

namespace HotelService.Interfaces
{
    public interface IHotelRepository
    {
        Task<IEnumerable<Hotel>> GetAllHotelsAsync(int page, int pageSize);
        Task<bool> AddHotelAsync(Hotel hotel);
        Task<bool> UpdateHotelAsync(Guid id, Hotel updatedHotel);
        Task<bool> DeleteHotelAsync(Guid id);
        Task<Hotel?> GetHotelByIdAsync(Guid id);
        Task<IEnumerable<Hotel>> GetHotelsByLocationAsync(string location);
    }

}