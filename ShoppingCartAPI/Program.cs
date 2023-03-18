using ShoppingCartAPI.Startup;
using ShoppingCartAPI.Models;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

ServicesSetup.AppSettingsConfigure(configuration);
ServicesSetup.SetJWTSettings(configuration.GetSection("JWT").Get<JWTModel>()!);

builder.Services.RegisterDependencyInjection();
builder.Services.RegisterDbContext();
builder.Services.RegisterIdentity();
builder.Services.RegisterAuthentication();
builder.Services.RegisterCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.ConfigureExceptionHandler();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
