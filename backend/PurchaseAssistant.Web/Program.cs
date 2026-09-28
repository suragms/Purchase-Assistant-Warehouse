using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Auth;
using PurchaseAssistant.Web.Authorization;
using PurchaseAssistant.Web.Services;
using System.Text;
using System.Reflection;
using PurchaseAssistant.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Settings & DI
builder.Services.Configure<JwtOptions>(options =>
{
    options.Issuer = "PurchaseAssistant";
    options.Audience = "PurchaseAssistantApp";
    options.SecretKey = "SuperSecretKeyForDevelopmentOnlyMakeSureToChangeInProduction12345!";
    options.ExpirationMinutes = 15;
});

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());
builder.Services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<CurrentUserService>());

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "PurchaseAssistant",
            ValidAudience = "PurchaseAssistantApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyForDevelopmentOnlyMakeSureToChangeInProduction12345!")),
            ClockSkew = System.TimeSpan.Zero
        };
    });

// Authorization Policies
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    // Define commonly used policies safely
    options.AddPolicy("RequireUsersView", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.UsersView)));
    options.AddPolicy("RequireUsersManage", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.UsersManage)));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", b =>
    {
        b.WithOrigins("http://localhost:5173", "http://localhost:3000") // standard vite/cra ports
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials();
    });
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        // Standardized 500 error mapping
        var response = new
        {
            error = new
            {
                code = "INTERNAL_SERVER_ERROR",
                message = "An unexpected error occurred.",
                details = exceptionHandlerPathFeature?.Error.Message,
                requestId = context.TraceIdentifier
            }
        };

        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();