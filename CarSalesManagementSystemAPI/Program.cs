using Repositories;
using Services;

namespace CarSalesManagementSystemAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
            builder.Services.AddScoped<IAppRoleRepository, AppRoleRepository>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IMaintenancePackageRepository, MaintenancePackageRepository>();
            builder.Services.AddScoped<IMaintenancePackageService, MaintenancePackageService>();
            builder.Services.AddScoped<IMaintenanceAppointmentRepository, MaintenanceAppointmentRepository>();
            builder.Services.AddScoped<IMaintenanceAppointmentService, MaintenanceAppointmentService>();

<<<<<<< Updated upstream
            builder.Services.AddControllers();
=======
            // Car Showroom flow registrations
            builder.Services.AddScoped<ICarRepository, CarRepository>();
            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddScoped<ICarBrandRepository, CarBrandRepository>();
            builder.Services.AddScoped<ICarBrandService, CarBrandService>();

            // Part flow registrations
            builder.Services.AddScoped<IPartCategoryRepository, PartCategoryRepository>();
            builder.Services.AddScoped<IPartCategoryService, PartCategoryService>();
            builder.Services.AddScoped<IPartRepository, PartRepository>();
            builder.Services.AddScoped<IPartService, PartService>();
            builder.Services.AddScoped<IPartOrderRepository, PartOrderRepository>();
            builder.Services.AddScoped<IPartOrderService, PartOrderService>();

            var modelBuilder = new ODataConventionModelBuilder();
            var cars = modelBuilder.EntitySet<BusinessObjects.Models.Car>("Cars");
            cars.EntityType.HasKey(c => c.CarId);

            var carBrands = modelBuilder.EntitySet<BusinessObjects.Models.CarBrand>("CarBrands");
            carBrands.EntityType.HasKey(cb => cb.BrandId);

            var packages = modelBuilder.EntitySet<BusinessObjects.Models.MaintenancePackage>("MaintenancePackages");
            packages.EntityType.HasKey(mp => mp.PackageId);

            var parts = modelBuilder.EntitySet<BusinessObjects.Models.Part>("Parts");
            parts.EntityType.HasKey(p => p.PartId);

            var partCategories = modelBuilder.EntitySet<BusinessObjects.Models.PartCategory>("PartCategories");
            partCategories.EntityType.HasKey(pc => pc.CategoryId);

            var partOrders = modelBuilder.EntitySet<BusinessObjects.Models.PartOrder>("PartOrders");
            partOrders.EntityType.HasKey(po => po.OrderId);

            builder.Services.AddControllers()
                .AddOData(options => options
                    .Select()
                    .Filter()
                    .OrderBy()
                    .Expand()
                    .Count()
                    .SetMaxTop(100)
                    .AddRouteComponents("odata", modelBuilder.GetEdmModel())
                )
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });
>>>>>>> Stashed changes
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // JWT Authentication Configuration
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
                };
            });

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            // Seed Default Roles
            using (var scope = app.Services.CreateScope())
            {
                var context = new DataAccessObjects.CarShowroomContext();
                if (!System.Linq.Enumerable.Any(context.AppRoles))
                {
                    context.AppRoles.AddRange(
                        new BusinessObjects.Models.AppRole { RoleName = "Admin" },
                        new BusinessObjects.Models.AppRole { RoleName = "Customer" }
                    );
                    context.SaveChanges();
                }
<<<<<<< Updated upstream
=======

                if (!System.Linq.Enumerable.Any(context.CarBrands))
                {
                    var toyota = new BusinessObjects.Models.CarBrand { BrandName = "Toyota", Country = "Japan", Description = "Toyota Motor Corporation" };
                    var ford = new BusinessObjects.Models.CarBrand { BrandName = "Ford", Country = "USA", Description = "Ford Motor Company" };
                    var vinfast = new BusinessObjects.Models.CarBrand { BrandName = "VinFast", Country = "Vietnam", Description = "VinFast Vietnam" };
                    var bmw = new BusinessObjects.Models.CarBrand { BrandName = "BMW", Country = "Germany", Description = "Bayerische Motoren Werke AG" };

                    context.CarBrands.AddRange(toyota, ford, vinfast, bmw);
                    context.SaveChanges();

                    if (!System.Linq.Enumerable.Any(context.Cars))
                    {
                        context.Cars.AddRange(
                            new BusinessObjects.Models.Car
                            {
                                BrandId = toyota.BrandId,
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
                                CreatedAt = DateTime.Now
                            },
                            new BusinessObjects.Models.Car
                            {
                                BrandId = toyota.BrandId,
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
                                CreatedAt = DateTime.Now
                            },
                            new BusinessObjects.Models.Car
                            {
                                BrandId = ford.BrandId,
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
                                CreatedAt = DateTime.Now
                            },
                            new BusinessObjects.Models.Car
                            {
                                BrandId = vinfast.BrandId,
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
                                CreatedAt = DateTime.Now
                            },
                            new BusinessObjects.Models.Car
                            {
                                BrandId = bmw.BrandId,
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
                                CreatedAt = DateTime.Now
                            },
                            new BusinessObjects.Models.Car
                            {
                                BrandId = vinfast.BrandId,
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
                                CreatedAt = DateTime.Now
                            }
                        );
                        context.SaveChanges();
                    }
                }

                if (!System.Linq.Enumerable.Any(context.PartCategories))
                {
                    var engine = new BusinessObjects.Models.PartCategory { CategoryName = "Động cơ & Truyền động", Description = "Các bộ phận liên quan đến động cơ, hộp số và truyền động." };
                    var electrical = new BusinessObjects.Models.PartCategory { CategoryName = "Hệ thống điện & Ắc quy", Description = "Ắc quy, máy phát điện, đèn và hệ thống điện." };
                    var fluids = new BusinessObjects.Models.PartCategory { CategoryName = "Dầu nhớt & Hóa chất", Description = "Dầu máy, nước làm mát, dầu phanh và hóa chất bảo dưỡng." };
                    var accessories = new BusinessObjects.Models.PartCategory { CategoryName = "Ngoại thất & Phụ kiện", Description = "Lốp xe, gạt mưa, gương và các phụ kiện trang trí ngoại thất." };

                    context.PartCategories.AddRange(engine, electrical, fluids, accessories);
                    context.SaveChanges();

                    if (!System.Linq.Enumerable.Any(context.Parts))
                    {
                        context.Parts.AddRange(
                            new BusinessObjects.Models.Part
                            {
                                CategoryId = accessories.CategoryId,
                                PartName = "Lốp xe Michelin Pilot Sport 4",
                                PartCode = "PT-MIC-PS4",
                                Brand = "Michelin",
                                Price = 3200000,
                                Quantity = 40,
                                Description = "Lốp hiệu năng cao, bám đường cực tốt trong mọi điều kiện thời tiết.",
                                ImageUrl = "https://images.unsplash.com/photo-1578844251758-2f71da64c96f?auto=format&fit=crop&w=600&q=80",
                                Status = "Available",
                                CreatedAt = DateTime.Now
                            },
                            new BusinessObjects.Models.Part
                            {
                                CategoryId = electrical.CategoryId,
                                PartName = "Ắc quy GS 12V 45Ah",
                                PartCode = "PT-GS-12V45",
                                Brand = "GS Battery",
                                Price = 1450000,
                                Quantity = 25,
                                Description = "Ắc quy khô miễn bảo dưỡng, độ bền cao, khởi động mạnh mẽ.",
                                ImageUrl = "https://images.unsplash.com/photo-1619642751034-765dfdf7c58e?auto=format&fit=crop&w=600&q=80",
                                Status = "Available",
                                CreatedAt = DateTime.Now
                            },
                            new BusinessObjects.Models.Part
                            {
                                CategoryId = fluids.CategoryId,
                                PartName = "Dầu nhớt Castrol Magnatec 5W-30",
                                PartCode = "PT-CAS-5W30",
                                Brand = "Castrol",
                                Price = 850000,
                                Quantity = 50,
                                Description = "Dầu nhớt công nghệ tổng hợp hoàn toàn bảo vệ động cơ ngay khi khởi động.",
                                ImageUrl = "https://images.unsplash.com/photo-1622560480605-d83c853bc5c3?auto=format&fit=crop&w=600&q=80",
                                Status = "Available",
                                CreatedAt = DateTime.Now
                            },
                            new BusinessObjects.Models.Part
                            {
                                CategoryId = accessories.CategoryId,
                                PartName = "Gạt mưa Bosch Aerotwin",
                                PartCode = "PT-BOS-AERO",
                                Brand = "Bosch",
                                Price = 450000,
                                Quantity = 60,
                                Description = "Gạt mưa cao cấp từ Bosch Đức, gạt sạch nước nhẹ nhàng, êm ái.",
                                ImageUrl = "https://images.unsplash.com/photo-1517524206127-48bbd363f3d7?auto=format&fit=crop&w=600&q=80",
                                Status = "Available",
                                CreatedAt = DateTime.Now
                            },
                            new BusinessObjects.Models.Part
                            {
                                CategoryId = electrical.CategoryId,
                                PartName = "Đèn pha LED Philips Ultinon Essential",
                                PartCode = "PT-PHI-LEDH7",
                                Brand = "Philips",
                                Price = 1200000,
                                Quantity = 15,
                                Description = "Bóng đèn LED H7 siêu sáng, gom sáng tốt, độ bền lên đến 5 năm.",
                                ImageUrl = "https://images.unsplash.com/photo-1508974239320-0a029497e820?auto=format&fit=crop&w=600&q=80",
                                Status = "Available",
                                CreatedAt = DateTime.Now
                            }
                        );
                        context.SaveChanges();
                    }
                }
>>>>>>> Stashed changes
            }

            app.Run();
        }
    }
}
