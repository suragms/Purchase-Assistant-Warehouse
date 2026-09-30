using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Infrastructure.Services;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Auth;
using PurchaseAssistant.Web.Authorization;
using PurchaseAssistant.Web.Services;
using System.Text;
using System.Reflection;
using PurchaseAssistant.Domain.Constants;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
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
builder.Services.AddScoped<IEntityNormalizationService, EntityNormalizationService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryTypeService, CategoryTypeService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IBrokerService, BrokerService>();
builder.Services.AddScoped<IGlobalSearchService, GlobalSearchService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());
builder.Services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<CurrentUserService>());

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
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
    options.AddPolicy("RequireCatalogView", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.CatalogView)));
    options.AddPolicy("RequireCatalogCreate", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.CatalogCreate)));
    options.AddPolicy("RequireCatalogEdit", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.CatalogEdit)));
    options.AddPolicy("RequireCatalogArchive", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.CatalogArchive)));
    options.AddPolicy("RequireSupplierView", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.SupplierView)));
    options.AddPolicy("RequireSupplierCreate", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.SupplierCreate)));
    options.AddPolicy("RequireSupplierEdit", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.SupplierEdit)));
    options.AddPolicy("RequireSupplierDelete", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.SupplierDelete)));
    options.AddPolicy("RequireBrokerView", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.BrokerView)));
    options.AddPolicy("RequireBrokerCreate", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.BrokerCreate)));
    options.AddPolicy("RequireBrokerEdit", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.BrokerEdit)));
    options.AddPolicy("RequireBrokerDelete", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.BrokerDelete)));
    options.AddPolicy("RequireStockView", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.StockView)));
    options.AddPolicy("RequireStockAdjust", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.StockAdjust)));
    options.AddPolicy("RequireStockPhysical", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.StockPhysical)));
    options.AddPolicy("RequireStockSystem", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.StockSystem)));
    options.AddPolicy("RequirePurchaseView", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.PurchaseView)));
    options.AddPolicy("RequirePurchaseCreate", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.PurchaseCreate)));
    options.AddPolicy("RequirePurchaseEdit", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.PurchaseEdit)));
    options.AddPolicy("RequirePurchaseDelete", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.PurchaseDelete)));
    options.AddPolicy("RequirePurchaseVerify", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.PurchaseVerify)));
    options.AddPolicy("RequirePurchaseCommit", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.PurchaseCommit)));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", b =>
    {
        b.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:3000")
         .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
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

// Ensure Database is migrated and seeded with default admin
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    try
    {
        await db.Database.MigrateAsync();

        if (!await db.Users.AnyAsync())
        {
            var business = new Business
            {
                Id = Guid.NewGuid(),
                Name = "Main Warehouse",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Businesses.Add(business);
            Console.WriteLine("Seeded initial business.");
        }

        // Update permissions for admin
        var adminUser = await db.Users.FirstOrDefaultAsync(u => u.Email == "admin@warehouse.local");
        if (adminUser != null)
        {
            var membership = await db.Memberships.FirstOrDefaultAsync(m => m.UserId == adminUser.Id);
            if (membership != null)
            {
                membership.PermissionsJson = JsonSerializer.Serialize(new[] {
                    "users.view", "users.manage",
                    "catalog.view", "catalog.manage",
                    "stock.view", "stock.manage",
                    "purchases.view", "purchases.manage",
                    "supplier.view", "supplier.create", "supplier.edit", "supplier.delete",
                    "broker.view", "broker.create", "broker.edit", "broker.delete",
                    "reports.view", "settings.manage"
                });
                await db.SaveChangesAsync();
                Console.WriteLine("Verified/Updated permissions for admin user.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DB Migration/Seed Warning: {ex.Message}");
    }
}

app.Run();