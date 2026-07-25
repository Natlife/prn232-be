using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Xunit;

namespace CarSalesManagementSystem.Tests
{
    public class CarsModuleTests
    {
        [Fact]
        public void Car_Status_Transitions_Should_Be_Valid()
        {
            var validStatuses = new[] { "Available", "Reserved", "Sold", "Inactive" };
            var car = new Car
            {
                CarId = 1,
                CarName = "VinFast VF9 Eco",
                Model = "VF9",
                Year = 2024,
                Price = 1491000000.00m,
                FuelType = "Electric",
                Transmission = "Automatic",
                Status = "Available"
            };

            Assert.Contains(car.Status, validStatuses);

            car.Status = "Reserved";
            Assert.Equal("Reserved", car.Status);

            car.Status = "Sold";
            Assert.Equal("Sold", car.Status);
        }

        [Fact]
        public void Car_Brand_Filtering_Should_Filter_Correctly()
        {
            var cars = new List<Car>
            {
                new Car { CarId = 1, BrandId = 1, CarName = "Toyota Camry", Price = 1350000000m, Status = "Available" },
                new Car { CarId = 2, BrandId = 2, CarName = "Ford Ranger", Price = 960000000m, Status = "Available" },
                new Car { CarId = 3, BrandId = 1, CarName = "Toyota Corolla Altis", Price = 750000000m, Status = "Available" }
            };

            var toyotaCars = cars.Where(c => c.BrandId == 1).ToList();

            Assert.Equal(2, toyotaCars.Count);
            Assert.All(toyotaCars, c => Assert.Equal(1, c.BrandId));
        }

        [Theory]
        [InlineData(1350000000.00, 1000000000.00, 1500000000.00, true)]
        [InlineData(4499000000.00, 1000000000.00, 3000000000.00, false)]
        [InlineData(960000000.00, 500000000.00, 1000000000.00, true)]
        public void Car_Price_Range_Filtering_Should_Be_Accurate(decimal price, decimal minPrice, decimal maxPrice, bool expectedInRange)
        {
            bool inRange = price >= minPrice && price <= maxPrice;

            Assert.Equal(expectedInRange, inRange);
        }

        [Fact]
        public void Car_Model_Initialization_Should_Preserve_All_Fields()
        {
            var car = new Car
            {
                CarId = 10,
                BrandId = 3,
                CarName = "BMW 730Li M Sport",
                Model = "730Li",
                Year = 2023,
                Color = "Black",
                Mileage = 12000,
                FuelType = "Gasoline",
                Transmission = "Automatic",
                Price = 4499000000.00m,
                Description = "Luxury sedan",
                ImageUrl = "https://images.unsplash.com/sample-bmw.jpg",
                Status = "Available"
            };

            Assert.Equal(10, car.CarId);
            Assert.Equal("BMW 730Li M Sport", car.CarName);
            Assert.Equal(4499000000.00m, car.Price);
            Assert.Equal("Available", car.Status);
        }
    }
}
