using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ShoppingCartAPI.Models;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace ShoppingCartAPI.Startup;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        CustomResponse response = _env.IsDevelopment() ? new CustomResponse(context.Response.StatusCode, ex.Message, ex.StackTrace!)
                                                       : new CustomResponse(context.Response.StatusCode, "Internal Server Error");

        string json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
