using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using DAL.Context;
using DAL.Entities;
using DTO.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interface;
using Repositories.Repository;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using ShoppingCartAPI.Models;
using System.Reflection;
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
        services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Mapper
        services.AddAutoMapper(typeof(Program));

        // Singleton Services
        services.AddSingleton<JWTModel>(_jwtModel);

        // Scoped Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped(typeof(IBaseEntityRepository<>), typeof(BaseEntityRepository<>));

        // Transient Services
        services.AddTransient<ExceptionMiddleware>();

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtModel.Secret)),
                ClockSkew = TimeSpan.FromSeconds(5)
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

    public static void ConfigureLogging()
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
        var configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",optional: true)
                            .Build();
        Log.Logger = new LoggerConfiguration()
                     .Enrich.FromLogContext()
                     .Enrich.WithExceptionDetails()
                     .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
                     .Enrich.WithProperty("Environment", environment)
                     .ReadFrom.Configuration(configuration)
                     .CreateLogger();
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
    {
        return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]!))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
            NumberOfReplicas = 1,
            NumberOfShards = 2
        };
    }
}
