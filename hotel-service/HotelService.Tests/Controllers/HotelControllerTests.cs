using HotelService.Controllers;
using HotelService.Interfaces;
using HotelService.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelService.Tests.Controllers
{
    public class HotelControllerTests
    {
        private readonly Mock<IHotelRepository> _mockHotelRepository;
        private readonly HotelController _controller;

        public HotelControllerTests()
        {
            _mockHotelRepository = new Mock<IHotelRepository>();
            _controller = new HotelController(_mockHotelRepository.Object);
        }

        [Fact]
        public async Task GetHotels_ReturnsOkResult_WithListOfHotels()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            _mockHotelRepository.Setup(repo => repo.GetAllHotelsAsync(page, pageSize))
                .ReturnsAsync(new List<Hotel> { new Hotel { Name = "Test Hotel", Location = "Test Location" } });

            // Act
            var result = await _controller.GetHotels(page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var hotels = Assert.IsType<List<Hotel>>(okResult.Value);
            Assert.Single(hotels);
        }

        [Fact]
        public async Task CreateHotel_ReturnsCreatedResult()
        {
            // Arrange
            var newHotel = new Hotel { Name = "New Hotel", Location = "New Location" };

            _mockHotelRepository.Setup(repo => repo.AddHotelAsync(newHotel)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateHotel(newHotel);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}
