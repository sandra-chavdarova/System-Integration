using EvolveDb;
using ExamsApplication.Domain.Models;
using ExamsApplication.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);
    options.UseLazyLoadingProxies();
});

builder.Services.AddIdentity<ExamsApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// TODO: Register Repository and Services Here

builder.Services.AddControllersWithViews();

using var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var logger = loggerFactory.CreateLogger("Evolve");

try
{
    using var cnx = new SqliteConnection(connectionString);

    var evolve = new Evolve(cnx, msg => logger.LogInformation(msg))
    {
        Locations = new[] { "Database/Migrations" },
        IsEraseDisabled = true
    };

    evolve.Migrate();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Database migration failed.");
    throw;
}

var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();