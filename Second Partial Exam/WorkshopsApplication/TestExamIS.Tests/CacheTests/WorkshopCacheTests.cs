using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestExamIS.Tests.Utils;

namespace TestExamIS.Tests.CacheTests;

[Collection("Test Suite")]
public class WorkshopCacheTests : LoggedTestBase, IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IMemoryCache _memoryCache;

    public WorkshopCacheTests(WebApplicationFactory<Program> factory, GlobalTestFixture fixture) : base(fixture)
    {
        _factory = factory.WithTestDatabase().WithTestAuth();
        _client = _factory.CreateAuthenticatedClient();
        _client.Timeout = TimeSpan.FromSeconds(5);
        _memoryCache = _factory.Services.GetRequiredService<IMemoryCache>();
    }

    public async Task InitializeAsync() => await TestDatabaseHelper.ResetDatabaseAsync(_factory.Services);
    public Task DisposeAsync() => Task.CompletedTask;

    [LoggedFact(Category = "Cache", Points = 5)]
    public async Task GetAll_WithVenueAndDateFilter_ShouldPopulateMemoryCache()
    {
        await RunTestAsync(async () =>
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var url = $"/api/Workshop?venueName=Hall-A&date={today:yyyy-MM-dd}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var cacheKey = $"workshops:Hall-A:{today:yyyy-MM-dd}";
            Assert.True(_memoryCache.TryGetValue(cacheKey, out _),
                $"Expected IMemoryCache to contain key '{cacheKey}' after GET {url}");
        });
    }

    [LoggedFact(Category = "Cache", Points = 5)]
    public async Task GetAll_NoFilter_ShouldPopulateMemoryCacheWithEmptyKey()
    {
        await RunTestAsync(async () =>
        {
            var response = await _client.GetAsync("/api/Workshop");
            response.EnsureSuccessStatusCode();

            var cacheKey = "workshops::";
            Assert.True(_memoryCache.TryGetValue(cacheKey, out _),
                $"Expected IMemoryCache to contain key '{cacheKey}' after GET /api/Workshop with no filters");
        });
    }

    [LoggedFact(Category = "Cache", Points = 5)]
    public async Task GetAll_CacheDuration_ShouldComeFromConfiguration()
    {
        await RunTestAsync(async () =>
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(typeof(Program).Assembly.Location)!)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var listDuration = config["CacheSettings:ListCacheDurationMinutes"];
            Assert.NotNull(listDuration);
            Assert.True(int.TryParse(listDuration, out var dur) && dur > 0,
                $"CacheSettings:ListCacheDurationMinutes must be a positive integer, got: '{listDuration}'");

            var response = await _client.GetAsync("/api/Workshop");
            response.EnsureSuccessStatusCode();

            Assert.True(_memoryCache.TryGetValue("workshops::", out _),
                "Cache must be populated using duration from CacheSettings:ListCacheDurationMinutes");
        });
    }
}
