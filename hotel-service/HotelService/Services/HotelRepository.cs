using HotelService.Data;
using HotelService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelService.Interfaces;

namespace HotelService.Services
{
    public class HotelRepository : IHotelRepository
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
                throw new ArgumentNullException(nameof(hotel), "Hotel information cannot be null.");

            if (string.IsNullOrWhiteSpace(hotel.Name) || string.IsNullOrWhiteSpace(hotel.Location))
                throw new ArgumentException("Hotel name and location information cannot be empty.");

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateHotelAsync(Guid id, Hotel updatedHotel)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException("The hotel to be updated was not found.");

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
                throw new KeyNotFoundException("The hotel to be deleted was not found.");

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Hotel?> GetHotelByIdAsync(Guid id)
        {
            return await _context.Hotels.FindAsync(id);
        }

        public async Task<IEnumerable<Hotel>> GetHotelsByLocationAsync(string location)
        {
            return await _context.Hotels
                .Where(h => h.Location == location)
                .ToListAsync();
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync(int page, int pageSize)
        {
            return await _context.Hotels
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

    }
}
