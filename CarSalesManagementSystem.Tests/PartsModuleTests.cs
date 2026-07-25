using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Xunit;

namespace CarSalesManagementSystem.Tests
{
    public class PartsModuleTests
    {
        [Fact]
        public void Part_Reorder_Threshold_Should_Detect_Low_Stock()
        {
            var parts = new List<Part>
            {
                new Part { PartId = 1, PartName = "Lốp xe Michelin", Quantity = 40, MinStockLevel = 10 },
                new Part { PartId = 2, PartName = "Bugi NGK", Quantity = 3, MinStockLevel = 15 },
                new Part { PartId = 3, PartName = "Lọc gió K&N", Quantity = 5, MinStockLevel = 5 }
            };

            var lowStockParts = parts.Where(p => p.Quantity <= p.MinStockLevel).ToList();

            Assert.Equal(2, lowStockParts.Count);
            Assert.Contains(lowStockParts, p => p.PartId == 2);
            Assert.Contains(lowStockParts, p => p.PartId == 3);
        }

        [Fact]
        public void Part_Total_Stock_Value_Calculation_Should_Be_Exact()
        {
            var part = new Part
            {
                PartId = 1,
                PartName = "Má phanh Brembo",
                PartCode = "PT-BREM-CERAMIC",
                Price = 4850000.00m,
                Quantity = 30
            };

            decimal totalStockValue = part.Price * part.Quantity;

            Assert.Equal(145500000.00m, totalStockValue);
        }

        [Fact]
        public void Part_UnitOfMeasure_And_Status_Defaults_Should_Be_Valid()
        {
            var part = new Part
            {
                CategoryId = 1,
                PartName = "Bugi Bạch Kim NGK",
                PartCode = "PT-NGK-IRIDIUM",
                Price = 350000m,
                Quantity = 100,
                UnitOfMeasure = "Cái",
                Status = "Available"
            };

            Assert.Equal("Cái", part.UnitOfMeasure);
            Assert.Equal("Available", part.Status);
        }
    }
}
