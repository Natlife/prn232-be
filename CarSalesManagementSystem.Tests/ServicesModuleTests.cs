using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.DTOs;
using BusinessObjects.Models;
using Xunit;

namespace CarSalesManagementSystem.Tests
{
    public class ServicesModuleTests
    {
        [Fact]
        public void MaintenancePackage_Price_Calculation_Should_Calculate_Savings()
        {
            var service1 = new Service { ServiceId = 1, ServiceName = "Thay dầu", BasePrice = 200000m, EstimatedDurationMinutes = 30 };
            var service2 = new Service { ServiceId = 2, ServiceName = "Vệ sinh dàn lạnh", BasePrice = 600000m, EstimatedDurationMinutes = 60 };

            var package = new MaintenancePackage
            {
                PackageId = 1,
                PackageName = "Gói Bảo Dưỡng VIP",
                PackagePrice = 700000m,
                Status = "Available"
            };

            decimal totalBasePrice = service1.BasePrice + service2.BasePrice;

            decimal savingAmount = totalBasePrice - package.PackagePrice;

            Assert.Equal(800000m, totalBasePrice);
            Assert.Equal(100000m, savingAmount);
            Assert.True(package.PackagePrice < totalBasePrice);
        }

        [Fact]
        public void MaintenanceAppointment_Service_Duration_Aggregation_Should_Calculate_Total_Time()
        {
            var services = new List<Service>
            {
                new Service { ServiceId = 1, EstimatedDurationMinutes = 30 },
                new Service { ServiceId = 2, EstimatedDurationMinutes = 45 },
                new Service { ServiceId = 3, EstimatedDurationMinutes = 60 }
            };

            int totalDurationMinutes = services.Sum(s => s.EstimatedDurationMinutes);

            Assert.Equal(135, totalDurationMinutes);
        }

        [Fact]
        public void AppointmentDetail_Subtotal_Calculation_Should_Be_Exact()
        {
            var detail = new AppointmentDetailDTO
            {
                AppointmentDetailId = 1,
                ServiceId = 3,
                ServiceName = "Phủ Ceramic 9H",
                UnitPrice = 5500000m,
                Quantity = 1
            };

            Assert.Equal(5500000m, detail.SubTotal);
        }

        [Fact]
        public void Service_DTO_Mapping_Should_Set_Default_Status_To_Available()
        {
            var serviceDto = new ServiceDTO
            {
                ServiceName = "Dán Phim Cách Nhiệt 3M",
                Description = "Chống nóng 99%",
                BasePrice = 12800000m,
                EstimatedDurationMinutes = 150
            };

            var service = new Service
            {
                ServiceName = serviceDto.ServiceName,
                Description = serviceDto.Description,
                BasePrice = serviceDto.BasePrice,
                EstimatedDurationMinutes = serviceDto.EstimatedDurationMinutes,
                Status = serviceDto.Status ?? "Available",
                CreatedAt = DateTime.Now
            };

            Assert.Equal("Available", service.Status);
            Assert.Equal(12800000m, service.BasePrice);
        }
    }
}
