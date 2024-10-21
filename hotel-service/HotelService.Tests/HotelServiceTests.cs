using HotelService.Controllers;
using HotelService.Data;
using HotelService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HotelService.Tests
{
    public class HotelServiceTests : IDisposable
    {
        private readonly HotelDbContext _context;
        private readonly HotelController _controller;
        private readonly CommunicationInfoController _communicationInfoController;

        public HotelServiceTests()
        {
            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseInMemoryDatabase(databaseName: "HotelServiceTestDb")
                .Options;

            _context = new HotelDbContext(options);
            _controller = new HotelController(_context);
            _communicationInfoController = new CommunicationInfoController(_context);

            // Db başta boş mu değil mi kontrol
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task CreateHotel_AddsHotelToDatabase()
        {
            // Arrange
            var hotel = new Hotel
            {
                Id = Guid.NewGuid(),
                Name = "Test Hotel",
                Location = "Test Location",
                ContactPersonFirstName = "Test",
                ContactPersonLastName = "Person",
                CompanyTitle = "Test Company",
                CommunicationInfos = new List<CommunicationInfo>
                {
                    new CommunicationInfo { InfoType = "Phone", InfoDetails = "123456789" }
                }
            };

            // Act
            var result = await _controller.CreateHotel(hotel);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);

            var createdHotel = await _context.Hotels.Include(h => h.CommunicationInfos).FirstOrDefaultAsync(h => h.Id == hotel.Id);
            Assert.NotNull(createdHotel);
            Assert.Single(createdHotel.CommunicationInfos); // İletişim bilgisinin eklenip eklenmediğini kontrol et
        }

        [Fact]
        public async Task GetHotelsByLocation_ReturnsHotels_WhenLocationExists()
        {
            // Arrange
            var hotel1 = new Hotel
            {
                Id = Guid.NewGuid(),
                Name = "Hotel One",
                Location = "Sakarya",
                ContactPersonFirstName = "Person 1",
                ContactPersonLastName = "Last 1",
                CompanyTitle = "Company 1"
            };

            var hotel2 = new Hotel
            {
                Id = Guid.NewGuid(),
                Name = "Hotel Two",
                Location = "Sakarya",
                ContactPersonFirstName = "Person 2",
                ContactPersonLastName = "Last 2",
                CompanyTitle = "Company 2"
            };

            var hotel3 = new Hotel
            {
                Id = Guid.NewGuid(),
                Name = "Hotel Three",
                Location = "Sakarya",
                ContactPersonFirstName = "Person 3",
                ContactPersonLastName = "Last 3",
                CompanyTitle = "Company 3"
            };

            await _context.Hotels.AddRangeAsync(hotel1, hotel2, hotel3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetHotelsByLocation("Sakarya");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var hotels = Assert.IsType<List<Hotel>>(okResult.Value);
            Assert.Equal(3, hotels.Count); // Beklenen 3 otel 
        }

        [Fact]
        public async Task AddCommunicationInfo_AddsCommunicationInfoToHotel()
        {
            // Arrange
            var hotel = new Hotel
            {
                Id = Guid.NewGuid(),
                Name = "Test Hotel",
                Location = "Test Location",
                ContactPersonFirstName = "Test",
                ContactPersonLastName = "Person",
                CompanyTitle = "Test Company"
            };

            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();

            var communicationInfo = new CommunicationInfoDto
            {
                InfoType = "Phone",
                InfoDetails = "123456789"
            };

            // Act
            var result = await _communicationInfoController.AddCommunicationInfo(hotel.Id, communicationInfo);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var updatedHotel = await _context.Hotels.Include(h => h.CommunicationInfos).FirstOrDefaultAsync(h => h.Id == hotel.Id);
            Assert.NotNull(updatedHotel);
            Assert.Single(updatedHotel.CommunicationInfos);
        }

        public void Dispose()
        {
            _context.Hotels.RemoveRange(_context.Hotels);
            _context.SaveChanges();
        }
    }
}
