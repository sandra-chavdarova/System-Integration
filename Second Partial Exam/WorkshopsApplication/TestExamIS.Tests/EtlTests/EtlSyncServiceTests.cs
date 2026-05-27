using System.Reflection;
using System.Text.Json;
using Domain.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Interface;
using TestExamIS.Tests.Utils;

namespace TestExamIS.Tests.EtlTests;

[Collection("Test Suite")]
public class EtlSyncServiceTests : LoggedTestBase, IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private EtlApiProxy? _apiProxy;

    public EtlSyncServiceTests(WebApplicationFactory<Program> factory, GlobalTestFixture fixture) : base(fixture)
    {
        _factory = factory
            .WithTestDatabase()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, cfg) =>
                    cfg.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["WorkshopsApiSettings:BaseAddress"] = "https://integriranisistemi.finki.ukim.mk/",
                        ["WorkshopsApiSettings:ApiKey"] = "kP7vXm2RwQs9TnY4LjB6HcDfE8gA1uZo"
                    }));

                builder.ConfigureTestServices(services =>
                {
                    var descriptor = services.FirstOrDefault(s =>
                        s.ServiceType.IsGenericType &&
                        s.ServiceType.GetGenericTypeDefinition() == typeof(IWorkshopsApiClient<>));

                    if (descriptor != null)
                    {
                        var serviceType = descriptor.ServiceType;
                        var createMethod = typeof(DispatchProxy)
                            .GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .Single(m => m.Name == nameof(DispatchProxy.Create)
                                         && m.IsGenericMethodDefinition
                                         && m.GetGenericArguments().Length == 2);
                        var proxy = (EtlApiProxy)createMethod
                            .MakeGenericMethod(serviceType, typeof(EtlApiProxy))
                            .Invoke(null, null)!;
                        _apiProxy = proxy;
                        services.Remove(descriptor);
                        services.AddScoped(serviceType, _ => proxy);
                    }
                });
            });
    }

    public async Task InitializeAsync()
    {
        await TestDatabaseHelper.ResetDatabaseAsync(_factory.Services);
        _apiProxy?.Reset(EmptyFeed());
    }
    public Task DisposeAsync() => Task.CompletedTask;

    private static string FeedJson(params (string externalId, string venueName, string title, DateTime start, DateTime end)[] items)
    {
        var arr = items.Select(x => new
        {
            externalId = x.externalId, venueName = x.venueName, title = x.title,
            startTime = x.start, endTime = x.end,
            lastModifiedUtc = DateTime.UtcNow, status = "Scheduled"
        });
        return JsonSerializer.Serialize(new
        {
            items = arr, page = 1, pageSize = 100,
            totalCount = items.Length, totalPages = 1, hasNextPage = false
        });
    }

    private static string EmptyFeed() =>
        JsonSerializer.Serialize(new
        {
            items = Array.Empty<object>(), page = 1, pageSize = 20,
            totalCount = 0, totalPages = 0, hasNextPage = false
        });

    private async Task RunEtl(Func<IEtlSyncService, Task> action)
    {
        using var scope = _factory.Services.CreateScope();
        var etl = scope.ServiceProvider.GetRequiredService<IEtlSyncService>();
        await action(etl);
    }

    [LoggedFact(Category = "EtlSync", Points = 10)]
    public async Task SyncAll_EmptyLog_ShouldImportAllWorkshops()
    {
        await RunTestAsync(async () =>
        {
            var venue = await TestDatabaseHelper.GetFirst<Venue>(_factory.Services);
            _apiProxy!.Json = FeedJson(
                ("EXT-TEST-001", venue.Name, "Imported Workshop 1", DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(30).AddHours(2)),
                ("EXT-TEST-002", venue.Name, "Imported Workshop 2", DateTime.UtcNow.AddDays(31), DateTime.UtcNow.AddDays(31).AddHours(1)));

            var countBefore = await TestDatabaseHelper.GetCount<Workshop>(_factory.Services);
            await RunEtl(e => e.SyncAllAsync());
            var countAfter = await TestDatabaseHelper.GetCount<Workshop>(_factory.Services);
            Assert.Equal(countBefore + 2, countAfter);
        });
    }

    [LoggedFact(Category = "EtlSync", Points = 5)]
    public async Task SyncAll_EmptyLog_ShouldCreateSuccessfulEtlSyncLogEntry()
    {
        await RunTestAsync(async () =>
        {
            await RunEtl(e => e.SyncAllAsync());
            var log = await TestDatabaseHelper.GetFirst<EtlSyncLog>(_factory.Services);
            Assert.NotNull(log);
            Assert.True(log.Success, "EtlSyncLog.Success must be true after successful sync");
            Assert.NotNull(log.CompletedAt);
        });
    }

    [LoggedFact(Category = "EtlSync", Points = 8)]
    public async Task SyncAll_EmptyLog_ShouldCallApiWithVeryOldDate()
    {
        await RunTestAsync(async () =>
        {
            await RunEtl(e => e.SyncAllAsync());
            Assert.NotNull(_apiProxy!.LastCalledWith);
            Assert.True(_apiProxy.LastCalledWith!.Value < new DateTime(2000, 1, 1),
                $"Initial sync must use DateTime.MinValue or pre-2000 date, got: {_apiProxy.LastCalledWith}");
        });
    }

    [LoggedFact(Category = "EtlSync", Points = 10)]
    public async Task SyncAll_WithExistingLog_ShouldCallApiWithLastSyncDate()
    {
        await RunTestAsync(async () =>
        {
            var lastSync = new DateTime(2060, 3, 26, 0, 0, 0, DateTimeKind.Utc);
            await TestDatabaseHelper.CreateEntity(_factory.Services, new EtlSyncLog
            {
                Id = Guid.NewGuid(), JobName = "WorkshopsSync",
                StartedAt = lastSync, CompletedAt = lastSync.AddMinutes(1), Success = true
            });

            await RunEtl(e => e.SyncAllAsync());
            Assert.NotNull(_apiProxy!.LastCalledWith);
            Assert.True(
                _apiProxy.LastCalledWith!.Value >= new DateTime(2060, 3, 26) &&
                _apiProxy.LastCalledWith.Value < new DateTime(2060, 3, 27),
                $"Incremental sync must use EtlSyncLog.StartedAt (2060-03-26), got: {_apiProxy.LastCalledWith}");
        });
    }

    [LoggedFact(Category = "EtlSync", Points = 10)]
    public async Task SyncAll_WithExistingLog_ShouldImportNewWorkshops()
    {
        await RunTestAsync(async () =>
        {
            var lastSync = new DateTime(2060, 3, 26, 0, 0, 0, DateTimeKind.Utc);
            await TestDatabaseHelper.CreateEntity(_factory.Services, new EtlSyncLog
            {
                Id = Guid.NewGuid(), JobName = "WorkshopsSync",
                StartedAt = lastSync, CompletedAt = lastSync.AddMinutes(1), Success = true
            });

            var venue = await TestDatabaseHelper.GetFirst<Venue>(_factory.Services);
            _apiProxy!.Json = FeedJson(
                ("INC-2060-001", venue.Name, "New Incremental WS", new DateTime(2061, 4, 1, 10, 0, 0), new DateTime(2061, 4, 1, 12, 0, 0)));

            var countBefore = await TestDatabaseHelper.GetCount<Workshop>(_factory.Services);
            await RunEtl(e => e.SyncAllAsync());
            var countAfter = await TestDatabaseHelper.GetCount<Workshop>(_factory.Services);
            Assert.Equal(countBefore + 1, countAfter);
        });
    }

    [LoggedFact(Category = "EtlSync", Points = 5)]
    public async Task SyncAll_ShouldNotDuplicateExistingWorkshops()
    {
        await RunTestAsync(async () =>
        {
            var venue = await TestDatabaseHelper.GetFirst<Venue>(_factory.Services);
            _apiProxy!.Json = FeedJson(
                ("EXT-DEDUP-001", venue.Name, "Dedup WS", DateTime.UtcNow.AddDays(40), DateTime.UtcNow.AddDays(40).AddHours(2)));

            await RunEtl(e => e.SyncAllAsync());
            var countAfterFirst = await TestDatabaseHelper.GetCount<Workshop>(_factory.Services);
            await RunEtl(e => e.SyncAllAsync());
            var countAfterSecond = await TestDatabaseHelper.GetCount<Workshop>(_factory.Services);
            Assert.Equal(countAfterFirst, countAfterSecond);
        });
    }

    [LoggedFact(Category = "EtlSync", Points = 5)]
    public async Task SyncAll_WhenApiFails_ShouldCreateEtlSyncLogWithSuccessFalse()
    {
        await RunTestAsync(async () =>
        {
            _apiProxy!.ShouldThrow = true;
            await RunEtl(e => e.SyncAllAsync());
            var log = await TestDatabaseHelper.GetFirst<EtlSyncLog>(_factory.Services);
            Assert.NotNull(log);
            Assert.False(log.Success, "EtlSyncLog.Success must be false when API call fails");
        });
    }

    [LoggedFact(Category = "EtlSync", Points = 5)]
    public async Task SyncAll_WhenApiFails_ShouldRecordErrorMessage()
    {
        await RunTestAsync(async () =>
        {
            _apiProxy!.ShouldThrow = true;
            await RunEtl(e => e.SyncAllAsync());
            var log = await TestDatabaseHelper.GetFirst<EtlSyncLog>(_factory.Services);
            Assert.NotNull(log);
            Assert.NotNull(log.ErrorMessage);
            Assert.NotEmpty(log.ErrorMessage!);
        });
    }

    internal class EtlApiProxy : DispatchProxy
    {
        internal string Json { get; set; } = "{}";
        internal DateTime? LastCalledWith { get; private set; }
        internal bool ShouldThrow { get; set; }

        internal void Reset(string json) { Json = json; LastCalledWith = null; ShouldThrow = false; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod?.Name != "GetAllWorkshopsModifiedSinceAsync") return null;
            LastCalledWith = (DateTime)args![0]!;
            if (ShouldThrow) throw new HttpRequestException("External API unavailable");
            var returnT = targetMethod.ReturnType.GetGenericArguments()[0];
            var deserialized = JsonSerializer.Deserialize(Json, returnT,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return typeof(Task).GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(returnT).Invoke(null, new[] { deserialized });
        }
    }
}
