using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Services;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Microsoft.EntityFrameworkCore;

namespace CarSalesManagementSystemAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
            builder.Services.AddScoped<IAppRoleRepository, AppRoleRepository>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IMaintenancePackageRepository, MaintenancePackageRepository>();
            builder.Services.AddScoped<IMaintenancePackageService, MaintenancePackageService>();
            builder.Services.AddScoped<IMaintenanceAppointmentRepository, MaintenanceAppointmentRepository>();
            builder.Services.AddScoped<IMaintenanceAppointmentService, MaintenanceAppointmentService>();
            builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<ICustomerCarRepository, CustomerCarRepository>();
            builder.Services.AddScoped<IAppointmentConsumedPartRepository, AppointmentConsumedPartRepository>();
            builder.Services.AddScoped<IAppointmentConsumedPartService, AppointmentConsumedPartService>();

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
            
            // New parts logistics flow registrations
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IPartCompatibilityRepository, PartCompatibilityRepository>();
            builder.Services.AddScoped<IPartCompatibilityService, PartCompatibilityService>();
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<IInventoryReceiptService, InventoryReceiptService>();
            // Deposit / Purchase flow
            builder.Services.AddScoped<IPurchaseRequestRepository, PurchaseRequestRepository>();
            builder.Services.AddScoped<IPurchaseRequestService, PurchaseRequestService>();
            builder.Services.AddScoped<IDepositCaptchaRepository, DepositCaptchaRepository>();
            builder.Services.AddScoped<IDepositCaptchaService, DepositCaptchaService>();
            // builder.Services.AddHostedService<DepositCleanupService>();

            // Combo Order stack
            // builder.Services.AddScoped<IComboOrderRepository, ComboOrderRepository>();
            // builder.Services.AddScoped<IComboOrderService, ComboOrderService>();

            // Chat proxy — delegates to Python RAG service
            builder.Services.AddHttpClient<IChatProxyService, ChatProxyService>();

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

            var purchaseRequests = modelBuilder.EntitySet<BusinessObjects.Models.PurchaseRequest>("PurchaseRequests");
            purchaseRequests.EntityType.HasKey(pr => pr.RequestId);

            var depositCaptchas = modelBuilder.EntitySet<BusinessObjects.Models.DepositCaptcha>("DepositCaptchas");
            depositCaptchas.EntityType.HasKey(dc => dc.CaptchaId);

            var odataSuppliers = modelBuilder.EntitySet<BusinessObjects.Models.Supplier>("Suppliers");
            odataSuppliers.EntityType.HasKey(s => s.SupplierId);

            builder.Services.AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

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

            // Automatically apply EF migrations and custom schemas on startup
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    System.Console.WriteLine("Entity Framework Migrations are managed manually via SQL script.");
                    using var context = new DataAccessObjects.CarShowroomContext();
                    // context.Database.Migrate();
                    // System.Console.WriteLine("EF Migrations applied successfully.");

                    // Check and apply custom deposit flow schema adjustments
                    var tableExists = false;
                    try
                    {
                        context.Database.ExecuteSqlRaw("SELECT TOP 1 1 FROM DepositCaptchas");
                        tableExists = true;
                    }
                    catch
                    {
                        // Table doesn't exist
                    }

                    if (!tableExists)
                    {
                        System.Console.WriteLine("Applying custom deposit migration schema...");
                        context.Database.ExecuteSqlRaw(@"
                            ALTER TABLE PurchaseRequests
                                ADD DepositAmount   DECIMAL(18,2)  NULL,
                                    DepositDate     DATETIME       NULL,
                                    DepositExpiry   DATETIME       NULL,
                                    CaptchaCode     NVARCHAR(20)   NULL;
                        ");

                        context.Database.ExecuteSqlRaw(@"
                            CREATE TABLE DepositCaptchas (
                                CaptchaId   INT IDENTITY(1,1) PRIMARY KEY,
                                Code        NVARCHAR(20) NOT NULL UNIQUE,
                                CarId       INT NOT NULL,
                                IsUsed      BIT NOT NULL DEFAULT 0,
                                CreatedAt   DATETIME NOT NULL DEFAULT GETDATE(),
                                UsedAt      DATETIME NULL,
                                CONSTRAINT FK_DepositCaptchas_Cars FOREIGN KEY (CarId) REFERENCES Cars(CarId)
                            );
                        ");
                        System.Console.WriteLine("Custom deposit migration schema applied successfully.");
                    }

                    // Check and apply custom delivery management schema adjustments
                    try
                    {
                        System.Console.WriteLine("Checking delivery schema columns in PartOrders...");
                        context.Database.ExecuteSqlRaw(@"
                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PartOrders') AND name = 'DeliveryMethod')
                            BEGIN
                                ALTER TABLE PartOrders
                                    ADD DeliveryMethod   NVARCHAR(50)   NOT NULL DEFAULT 'Pickup',
                                        ShippingFee      DECIMAL(18,2)  NOT NULL DEFAULT 0;
                            END
                        ");
                        context.Database.ExecuteSqlRaw(@"
                            ALTER TABLE PartOrders
                                ALTER COLUMN ShippingAddress NVARCHAR(255) NULL;
                        ");
                        
                        System.Console.WriteLine("Checking IsPaid column in MaintenanceAppointments...");
                        context.Database.ExecuteSqlRaw(@"
                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MaintenanceAppointments') AND name = 'IsPaid')
                            BEGIN
                                ALTER TABLE MaintenanceAppointments
                                    ADD IsPaid BIT NOT NULL DEFAULT 0;
                            END
                        ");
                        
                        System.Console.WriteLine("Schema checks completed.");
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine($"Error modifying PartOrders schema: {ex.InnerException?.Message ?? ex.Message}");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Error running migrations: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
