using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            builder.Services.AddControllers(options => 
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
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
            }

            app.Run();
        }
    }
}
