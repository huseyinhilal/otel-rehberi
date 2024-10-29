using HotelService.Models;

namespace HotelService.Interfaces
{
    public interface IHotelRepository
    {
        Task<bool> AddHotelAsync(Hotel hotel);
        Task<bool> UpdateHotelAsync(Guid id, Hotel updatedHotel);
        Task<bool> DeleteHotelAsync(Guid id);
        Task<IEnumerable<Hotel>> GetAllHotelsAsync();
    }
}
