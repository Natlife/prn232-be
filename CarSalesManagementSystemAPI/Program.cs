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

            // Car Showroom flow registrations
            builder.Services.AddScoped<ICarRepository, CarRepository>();
            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddScoped<ICarBrandRepository, CarBrandRepository>();
            builder.Services.AddScoped<ICarBrandService, CarBrandService>();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });
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

            // Seed Default Roles, Brands and Cars
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
            }

            app.Run();
        }
    }
}
