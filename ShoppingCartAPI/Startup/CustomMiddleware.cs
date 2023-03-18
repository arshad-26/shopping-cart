using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ShoppingCartAPI.Models;
using System.Net;

namespace ShoppingCartAPI.Startup;

public static class CustomMiddleware
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app){
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Internal Server Error."
                    }.ToString());
                }
            });
        });
    }
}
