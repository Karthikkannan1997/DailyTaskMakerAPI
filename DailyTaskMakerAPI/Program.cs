using DailyTaskMaker.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Graph.ExternalConnectors;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using DailyTaskMaker.Infrastructure.Interfaces;
using DailyTaskMaker.Infrastructure.Repository;
using DailyTaskMakerAPI.Extensions;
using System.Security.Claims;
using DailyTaskMakerAPI.Middleware;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins(["http://localhost:4200", "https://daily-task-maker-app-cmg8hnbvckedgjgu.southindia-01.azurewebsites.net"]) // Replace with your frontend URL
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddScoped<GraphServiceClient>(serviceProvider =>
//{
//    var tokenAcquisition = serviceProvider.GetRequiredService<ITokenAcquisition>();
//    return new GraphServiceClient(new DelegateAuthenticationProvider(async requestMessage =>
//    {
//        var token = await tokenAcquisition.GetAccessTokenForUserAsync(new[] { "User.Read" });
//        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
//    }));
//});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddConfigureServices(builder.Configuration);



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<RoleFetchingMiddleware>();// Use the custom middleware
app.UseAuthorization();
app.MapControllers();

app.Run();
