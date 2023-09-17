using DTO.Identity;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using ShoppingCartAPI.Startup;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

ServicesSetup.AppSettingsConfigure(configuration);
ServicesSetup.SetJWTSettings(configuration.GetSection("JWT").Get<JWTModel>()!);

builder.Services.RegisterDependencyInjection();
builder.Services.RegisterDbContext();
builder.Services.RegisterIdentity();
builder.Services.RegisterAuthentication();
builder.Services.RegisterCors();

ServicesSetup.ConfigureLogging();
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();