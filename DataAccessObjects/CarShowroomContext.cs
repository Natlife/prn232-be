using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using BusinessObjects.Models;

namespace DataAccessObjects;

public partial class CarShowroomContext : DbContext
{
    public CarShowroomContext()
    {
    }

    public CarShowroomContext(DbContextOptions<CarShowroomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppRole> AppRoles { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarBrand> CarBrands { get; set; }

    public virtual DbSet<MaintenanceAppointment> MaintenanceAppointments { get; set; }

    public virtual DbSet<MaintenancePackage> MaintenancePackages { get; set; }

    public virtual DbSet<Part> Parts { get; set; }

    public virtual DbSet<PartCategory> PartCategories { get; set; }

    public virtual DbSet<PartOrder> PartOrders { get; set; }

    public virtual DbSet<PartOrderDetail> PartOrderDetails { get; set; }

    public virtual DbSet<PurchaseRequest> PurchaseRequests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__AppRoles__8AFACE1ABED980DF");

            entity.HasIndex(e => e.RoleName, "UQ__AppRoles__8A2B6160E1219BDE").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);

            entity.HasData(
                new AppRole { RoleId = 1, RoleName = "Admin" },
                new AppRole { RoleId = 2, RoleName = "Customer" }
            );
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__AppUsers__1788CC4CD3D16938");

            entity.HasIndex(e => e.Email, "UQ__AppUsers__A9D10534CD643903").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Role).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppUsers_AppRoles");

            entity.HasData(
                new AppUser
                {
                    UserId = 1,
                    FullName = "System Admin",
                    Email = "admin@gmail.com",
                    PasswordHash = "$2a$11$ivuFcskipHfVJyUk7X7Cy.72DYWJAKQhFt7uaF2kMrwZ/LAHW1cWO", // password: admin
                    PhoneNumber = "0987654321",
                    Address = "Hanoi",
                    RoleId = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new AppUser
                {
                    UserId = 2,
                    FullName = "John Customer",
                    Email = "customer@gmail.com",
                    PasswordHash = "$2a$11$iR0JU.l1mLeRCyKuClJFxuWqtweaw2kS3oZSRG/lAcD00M603P5Mm", // password: customer
                    PhoneNumber = "0123456789",
                    Address = "HCM City",
                    RoleId = 2,
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Cars__68A0342E9C46E9E9");

            entity.Property(e => e.CarName).HasMaxLength(150);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.FuelType).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");
            entity.Property(e => e.Transmission).HasMaxLength(50);

            entity.HasOne(d => d.Brand).WithMany(p => p.Cars)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cars_CarBrands");

            entity.HasData(
                new Car
                {
                    CarId = 1,
                    BrandId = 1,
                    CarName = "Toyota Camry 2.5Q",
                    Model = "Camry",
                    Year = 2022,
                    Color = "Black",
                    Mileage = 15000,
                    FuelType = "Gasoline",
                    Transmission = "Automatic",
                    Price = 1350000000,
                    Description = "Xe sang trọng, lịch lãm, gia đình sử dụng kỹ, bảo dưỡng chính hãng.",
                    ImageUrl = "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Car
                {
                    CarId = 2,
                    BrandId = 1,
                    CarName = "Toyota Vios 1.5G",
                    Model = "Vios",
                    Year = 2021,
                    Color = "White",
                    Mileage = 28000,
                    FuelType = "Gasoline",
                    Transmission = "Automatic",
                    Price = 520000000,
                    Description = "Xe quốc dân tiết kiệm nhiên liệu, vận hành bền bỉ.",
                    ImageUrl = "https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Car
                {
                    CarId = 3,
                    BrandId = 2,
                    CarName = "Ford Ranger Wildtrak 2.0L",
                    Model = "Ranger",
                    Year = 2023,
                    Color = "Orange",
                    Mileage = 8000,
                    FuelType = "Diesel",
                    Transmission = "Automatic",
                    Price = 960000000,
                    Description = "Vua bán tải, phiên bản cao cấp nhất Wildtrak 2 cầu, đầy đủ công nghệ.",
                    ImageUrl = "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Car
                {
                    CarId = 4,
                    BrandId = 3,
                    CarName = "VinFast VF8 Plus",
                    Model = "VF8",
                    Year = 2023,
                    Color = "Blue",
                    Mileage = 5000,
                    FuelType = "Electric",
                    Transmission = "Automatic",
                    Price = 1100000000,
                    Description = "Xe điện thông minh Việt Nam, bản Plus pin SDI, công nghệ ADAS hiện đại.",
                    ImageUrl = "https://images.unsplash.com/photo-1563720223185-11003d516935?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Car
                {
                    CarId = 5,
                    BrandId = 4,
                    CarName = "BMW 320i Sport Line",
                    Model = "3 Series",
                    Year = 2020,
                    Color = "Red",
                    Mileage = 35000,
                    FuelType = "Gasoline",
                    Transmission = "Automatic",
                    Price = 1250000000,
                    Description = "Dòng sedan thể thao lái cực hay, ngoại hình trẻ trung năng động.",
                    ImageUrl = "https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Car
                {
                    CarId = 6,
                    BrandId = 3,
                    CarName = "VinFast VF5 Plus",
                    Model = "VF5",
                    Year = 2023,
                    Color = "Gray",
                    Mileage = 2000,
                    FuelType = "Electric",
                    Transmission = "Automatic",
                    Price = 450000000,
                    Description = "Xe đô thị cỡ nhỏ thông minh, cực kỳ tiết kiệm và nhỏ gọn.",
                    ImageUrl = "https://images.unsplash.com/photo-1617788138017-80ad40651399?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );
        });

        modelBuilder.Entity<CarBrand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__CarBrand__DAD4F05EFE11BDE9");

            entity.Property(e => e.BrandName).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasData(
                new CarBrand { BrandId = 1, BrandName = "Toyota", Country = "Japan", Description = "Toyota Motor Corporation" },
                new CarBrand { BrandId = 2, BrandName = "Ford", Country = "USA", Description = "Ford Motor Company" },
                new CarBrand { BrandId = 3, BrandName = "VinFast", Country = "Vietnam", Description = "VinFast Vietnam" },
                new CarBrand { BrandId = 4, BrandName = "BMW", Country = "Germany", Description = "Bayerische Motoren Werke AG" }
            );
        });

        modelBuilder.Entity<MaintenanceAppointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Maintena__8ECDFCC2C25D6E2B");

            entity.Property(e => e.CarName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerEmail).HasMaxLength(100);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.LicensePlate).HasMaxLength(30);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.MaintenanceAppointments)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintenanceAppointments_AppUsers");

            entity.HasOne(d => d.Package).WithMany(p => p.MaintenanceAppointments)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintenanceAppointments_Packages");
        });

        modelBuilder.Entity<MaintenancePackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__Maintena__322035CCFD54E0FD");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.PackageName).HasMaxLength(150);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");

            entity.HasData(
                new MaintenancePackage
                {
                    PackageId = 1,
                    PackageName = "Bảo dưỡng Tiêu chuẩn",
                    Description = "Kiểm tra toàn diện 30 điểm, thay nhớt động cơ và lọc nhớt, kiểm tra hệ thống phanh và bổ sung nước làm mát. Phù hợp cho bảo dưỡng định kỳ mỗi 5.000 km.",
                    Price = 1500000,
                    EstimatedDuration = 120,
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new MaintenancePackage
                {
                    PackageId = 2,
                    PackageName = "Bảo dưỡng Toàn diện VIP",
                    Description = "Kiểm tra hệ thống điện tử bằng máy chuyên dụng, vệ sinh buồng đốt, vệ sinh kim phun, đảo lốp, cân bằng động và thay toàn bộ chất lỏng (dầu máy, dầu phanh, nước làm mát).",
                    Price = 4500000,
                    EstimatedDuration = 240,
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new MaintenancePackage
                {
                    PackageId = 3,
                    PackageName = "Kiểm tra Xe trước Chuyến đi",
                    Description = "Kiểm tra áp suất lốp, độ mòn lốp, hệ thống chiếu sáng, hệ thống phanh, gạt mưa và bình ắc quy để đảm bảo an toàn tuyệt đối cho chuyến đi dài.",
                    Price = 500000,
                    EstimatedDuration = 60,
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.PartId).HasName("PK__Parts__7C3F0D509A440590");

            entity.HasIndex(e => e.PartCode, "UQ__Parts__6525D39D6EAC6A52").IsUnique();

            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.PartCode).HasMaxLength(50);
            entity.Property(e => e.PartName).HasMaxLength(150);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");

            entity.HasOne(d => d.Category).WithMany(p => p.Parts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Parts_PartCategories");

            entity.HasData(
                new Part
                {
                    PartId = 1,
                    CategoryId = 4,
                    PartName = "Lốp xe Michelin Pilot Sport 4",
                    PartCode = "PT-MIC-PS4",
                    Brand = "Michelin",
                    Price = 3200000,
                    Quantity = 40,
                    Description = "Lốp hiệu năng cao, bám đường cực tốt trong mọi điều kiện thời tiết.",
                    ImageUrl = "https://images.unsplash.com/photo-1578844251758-2f71da64c96f?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Part
                {
                    PartId = 2,
                    CategoryId = 2,
                    PartName = "Ắc quy GS 12V 45Ah",
                    PartCode = "PT-GS-12V45",
                    Brand = "GS Battery",
                    Price = 1450000,
                    Quantity = 25,
                    Description = "Ắc quy khô miễn bảo dưỡng, độ bền cao, khởi động mạnh mẽ.",
                    ImageUrl = "https://images.unsplash.com/photo-1619642751034-765dfdf7c58e?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Part
                {
                    PartId = 3,
                    CategoryId = 3,
                    PartName = "Dầu nhớt Castrol Magnatec 5W-30",
                    PartCode = "PT-CAS-5W30",
                    Brand = "Castrol",
                    Price = 850000,
                    Quantity = 50,
                    Description = "Dầu nhớt công nghệ tổng hợp hoàn toàn bảo vệ động cơ ngay khi khởi động.",
                    ImageUrl = "https://images.unsplash.com/photo-1622560480605-d83c853bc5c3?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Part
                {
                    PartId = 4,
                    CategoryId = 4,
                    PartName = "Gạt mưa Bosch Aerotwin",
                    PartCode = "PT-BOS-AERO",
                    Brand = "Bosch",
                    Price = 450000,
                    Quantity = 60,
                    Description = "Gạt mưa cao cấp từ Bosch Đức, gạt sạch nước nhẹ nhàng, êm ái.",
                    ImageUrl = "https://images.unsplash.com/photo-1517524206127-48bbd363f3d7?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Part
                {
                    PartId = 5,
                    CategoryId = 2,
                    PartName = "Đèn pha LED Philips Ultinon Essential",
                    PartCode = "PT-PHI-LEDH7",
                    Brand = "Philips",
                    Price = 1200000,
                    Quantity = 15,
                    Description = "Bóng đèn LED H7 siêu sáng, gom sáng tốt, độ bền lên đến 5 năm.",
                    ImageUrl = "https://images.unsplash.com/photo-1508974239320-0a029497e820?auto=format&fit=crop&w=600&q=80",
                    Status = "Available",
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );
        });

        modelBuilder.Entity<PartCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__PartCate__19093A0B38EB3018");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasData(
                new PartCategory { CategoryId = 1, CategoryName = "Động cơ & Truyền động", Description = "Các bộ phận liên quan đến động cơ, hộp số và truyền động." },
                new PartCategory { CategoryId = 2, CategoryName = "Hệ thống điện & Ắc quy", Description = "Ắc quy, máy phát điện, đèn và hệ thống điện." },
                new PartCategory { CategoryId = 3, CategoryName = "Dầu nhớt & Hóa chất", Description = "Dầu máy, nước làm mát, dầu phanh và hóa chất bảo dưỡng." },
                new PartCategory { CategoryId = 4, CategoryName = "Ngoại thất & Phụ kiện", Description = "Lốp xe, gạt mưa, gương và các phụ kiện trang trí ngoại thất." }
            );
        });

        modelBuilder.Entity<PartOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__PartOrde__C3905BCF6A12AD11");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerEmail).HasMaxLength(100);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.ShippingAddress).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.PartOrders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartOrders_AppUsers");
        });

        modelBuilder.Entity<PartOrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__PartOrde__D3B9D36CC532A696");

            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.PartOrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartOrderDetails_PartOrders");

            entity.HasOne(d => d.Part).WithMany(p => p.PartOrderDetails)
                .HasForeignKey(d => d.PartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartOrderDetails_Parts");
        });

        modelBuilder.Entity<PurchaseRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Purchase__33A8517AB622A517");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerEmail).HasMaxLength(100);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Car).WithMany(p => p.PurchaseRequests)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseRequests_Cars");

            entity.HasOne(d => d.Customer).WithMany(p => p.PurchaseRequests)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseRequests_AppUsers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}