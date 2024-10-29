using HotelService.Data;
using HotelService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Services
{
    public class HotelRepository
    {
        private readonly HotelDbContext _context;

        public HotelRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
        {
            return await _context.Hotels.ToListAsync();
        }

        public async Task<bool> AddHotelAsync(Hotel hotel)
        {
            if (hotel == null)
                throw new ArgumentNullException(nameof(hotel), "Hotel bilgisi boş olamaz.");

            if (string.IsNullOrWhiteSpace(hotel.Name) || string.IsNullOrWhiteSpace(hotel.Location))
                throw new ArgumentException("Hotel adı ve lokasyon bilgisi boş olamaz.");

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateHotelAsync(Guid id, Hotel updatedHotel)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException("Güncellenmek istenen otel bulunamadı.");

            hotel.Name = updatedHotel.Name;
            hotel.Location = updatedHotel.Location;
            hotel.ContactPersonFirstName = updatedHotel.ContactPersonFirstName;
            hotel.ContactPersonLastName = updatedHotel.ContactPersonLastName;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHotelAsync(Guid id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException("Silinmek istenen otel bulunamadı.");

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
