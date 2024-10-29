using HotelService.Controllers;
using HotelService.Interfaces;
using HotelService.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HotelService.Tests
{
    public class HotelServiceTests
    {
        private readonly Mock<IHotelRepository> _mockHotelRepository;
        private readonly HotelController _controller;

        public HotelServiceTests()
        {
            _mockHotelRepository = new Mock<IHotelRepository>();
            _controller = new HotelController(_mockHotelRepository.Object);
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
                CompanyTitle = "Test Company"
            };

            _mockHotelRepository.Setup(repo => repo.AddHotelAsync(hotel)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateHotel(hotel);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            _mockHotelRepository.Verify(repo => repo.AddHotelAsync(hotel), Times.Once);
        }

        [Fact]
        public async Task GetHotelsByLocation_ReturnsHotels_WhenLocationExists()
        {
            // Arrange
            var location = "Sakarya";
            var hotels = new List<Hotel>
            {
                new Hotel { Id = Guid.NewGuid(), Name = "Hotel One", Location = location, ContactPersonFirstName = "Person 1", ContactPersonLastName = "Last 1", CompanyTitle = "Company 1" },
                new Hotel { Id = Guid.NewGuid(), Name = "Hotel Two", Location = location, ContactPersonFirstName = "Person 2", ContactPersonLastName = "Last 2", CompanyTitle = "Company 2" },
                new Hotel { Id = Guid.NewGuid(), Name = "Hotel Three", Location = location, ContactPersonFirstName = "Person 3", ContactPersonLastName = "Last 3", CompanyTitle = "Company 3" }
            };

            _mockHotelRepository.Setup(repo => repo.GetHotelsByLocationAsync(location)).ReturnsAsync(hotels);

            // Act
            var result = await _controller.GetHotelsByLocation(location);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedHotels = Assert.IsType<List<Hotel>>(okResult.Value);
            Assert.Equal(3, returnedHotels.Count);
        }
    }
}
