using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartAPI.Models;
using System.Text;

namespace ShoppingCartAPI.Startup;

public static class ServicesSetup
{
    #nullable disable

    private static IConfiguration _config;
    private static JWTModel _jwtModel;

    #nullable restore

    public static void AppSettingsConfigure(IConfiguration config) => _config = config;

    public static void SetJWTSettings(JWTModel jWTModel) => _jwtModel = jWTModel;

    public static IServiceCollection RegisterDependencyInjection(this IServiceCollection services)
    {
        // Default Services
        services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Singleton Services
        services.AddSingleton<JWTModel>(_jwtModel);

        return services;        
    }

    public static IServiceCollection RegisterDbContext(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options => {
            options.UseSqlServer(_config.GetConnectionString("Default"));
        });

        return services;
    }

    public static IServiceCollection RegisterIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;

            options.User.RequireUniqueEmail = true;

            options.Lockout.AllowedForNewUsers = false;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection RegisterAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidAudience = _jwtModel.ValidAudience,
                ValidIssuer = _jwtModel.ValidIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtModel.Secret))
            };
        });

        return services;
    }

    public static IServiceCollection RegisterCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsBuilder =>
            {
                corsBuilder.WithOrigins(_config["API_HOST"]!).AllowAnyHeader().AllowAnyMethod();
            });
        });

        return services;
    }
}
