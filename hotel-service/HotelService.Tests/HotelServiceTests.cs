using Xunit;
using HotelService.Controllers;
using HotelService.Data;
using HotelService.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


public class HotelServiceTests
{
    private HotelController _controller;
    private HotelDbContext _context;

    public HotelServiceTests()
    {
        // In-memory veritabanı kullanarak test için bir context oluşturuyoruz
        var options = new DbContextOptionsBuilder<HotelDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new HotelDbContext(options);
        _controller = new HotelController(_context);
    }

    [Fact]
    public async Task GetHotels_ReturnsEmptyList_WhenNoHotelsExist()
    {
        // Act
        var result = await _controller.GetHotels();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var hotels = Assert.IsType<List<Hotel>>(okResult.Value);
        Assert.Empty(hotels);
    }

    [Fact]
    public async Task CreateHotel_AddsHotelToDatabase()
    {
        // Arrange
        var newHotel = new Hotel
        {
            Name = "Test Hotel",
            Location = "Test Location",
            ContactPersonFirstName = "Test Person Name",
            ContactPersonLastName = "Test Person Lastname",
            ContactInfo = "test@example.com",
            CompanyTitle = "Test Company"
        };

        // Act
        var result = await _controller.CreateHotel(newHotel);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var hotel = Assert.IsType<Hotel>(createdAtActionResult.Value);
        Assert.Equal("Test Hotel", hotel.Name);
    }

    // Diğer testler: güncelleme, silme işlemleri için benzer testler yazabiliriz
}
