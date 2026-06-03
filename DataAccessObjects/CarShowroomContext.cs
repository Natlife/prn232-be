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
                    UserId = 99, 
                    FullName = "System Admin", 
                    Email = "admin@group7.com", 
                    PasswordHash = "$2a$11$jI66yMj5egkHkF0.UF41C.Wls0ZtKv2WbVNqemyJvJjo7OlT8r.4u", // Admin@123
                    PhoneNumber = "0987654321",
                    RoleId = 1, 
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
        });

        modelBuilder.Entity<CarBrand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__CarBrand__DAD4F05EFE11BDE9");

            entity.Property(e => e.BrandName).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
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
        });

        modelBuilder.Entity<PartCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__PartCate__19093A0B38EB3018");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
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
