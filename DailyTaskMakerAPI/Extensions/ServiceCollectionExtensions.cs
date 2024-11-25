using DailyTaskMaker.Infrastructure.Interfaces;
using DailyTaskMaker.Infrastructure.Models;
using DailyTaskMaker.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace DailyTaskMakerAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Daily Task Maker API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
        });
            });
            services.AddDbContext<DailyTaskMakerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Add HttpContextAccessor
            services.AddHttpContextAccessor();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
                options.AddPolicy("PremiumPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Premium User"));
                options.AddPolicy("FreePolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Free User"));
            });
            // Add custom services
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IExpenseTrackerRepository, ExpenseTrackerRepository>();
            services.AddScoped<IUserManagementRepository, UserManagementRepository>();
            return services;
        }
    }
}
