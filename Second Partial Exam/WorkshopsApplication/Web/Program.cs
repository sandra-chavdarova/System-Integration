using System.Threading.Channels;
using System.Threading.RateLimiting;
using Domain.Config;
using Domain.Dto;
using Domain.Dto.Email;
using Domain.Models;
using EvolveDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repository;
using Repository.Implementation;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;
using Service.Jobs;
using Web.DbSeeder;
using Web.Mapper;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);
    options.UseLazyLoadingProxies();
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IWorkshopService, WorkshopService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<WorkshopMapper>();
builder.Services.AddScoped<EnrollmentMapper>();
builder.Services.AddScoped<IWorkshopsRepository, WorkshopsRepository>();
builder.Services.AddScoped<IInboundEventEntryService, InboundEventEntryService>();

builder.Services.AddScoped<IInboundEventEntryProcessor, InboundEventEntryProcessor>();
builder.Services.AddScoped<IEtlSyncService, EtlSyncService>();


builder.Services.AddHttpClient<IWorkshopsApiClient<ExternalWorkshopsDto>, WorkshopsApiClient>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<WorkshopsApiSettings>>();

    client.BaseAddress = new Uri(settings.Value.BaseAddress);
    client.DefaultRequestHeaders.Add("X-Api-Key", settings.Value.ApiKey);
});

builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));
builder.Services.Configure<RateLimitSettings>(builder.Configuration.GetSection("RateLimitSettings"));
builder.Services.Configure<ApiKeySettings>(builder.Configuration.GetSection("ApiKeySettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<WorkshopsApiSettings>(builder.Configuration.GetSection("WorkshopsApiSettings"));

builder.Services.AddHostedService<BackgroundEnrollmentEntryService>();
builder.Services.AddHostedService<SyncWorkshopsBackgroundService>();
builder.Services.AddHostedService<EmailBackgroundService>();

builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddSingleton<IEmailQueue, ChannelEmailQueue>();
builder.Services.AddSingleton(Channel.CreateUnbounded<EmailMessage>());


builder.Services.AddMemoryCache();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;

    options.AddPolicy("external-api", context =>
    {
        var settings = context.RequestServices.GetRequiredService<IOptions<RateLimitSettings>>().Value;
        var apiKey = context.Request.Headers["x-api-key"];

        return RateLimitPartition.GetFixedWindowLimiter(apiKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = settings.PermitLimit,
            QueueLimit = 0,
            Window = TimeSpan.FromSeconds(settings.WindowInSeconds),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });
});


builder.Services.AddIdentity<WorkshopApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

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
        { Locations = new[] { "Database/Migrations" }, IsEraseDisabled = true };
    evolve.Migrate();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Database migration failed.");
    throw;
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<WorkshopApplicationUser>>();
    await DbSeeder.SeedAsync(context, userManager);
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// TODO: Add UseRateLimiter()

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseRateLimiter();

app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();

app.Run();

public partial class Program
{
}