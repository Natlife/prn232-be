using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Services;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

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
            // Deposit / Purchase flow
            builder.Services.AddScoped<IPurchaseRequestRepository, PurchaseRequestRepository>();
            builder.Services.AddScoped<IPurchaseRequestService, PurchaseRequestService>();
            builder.Services.AddScoped<IDepositCaptchaRepository, DepositCaptchaRepository>();
            builder.Services.AddScoped<IDepositCaptchaService, DepositCaptchaService>();
            builder.Services.AddHostedService<DepositCleanupService>();

            // Combo Order stack
            builder.Services.AddScoped<IComboOrderRepository, ComboOrderRepository>();
            builder.Services.AddScoped<IComboOrderService, ComboOrderService>();

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
