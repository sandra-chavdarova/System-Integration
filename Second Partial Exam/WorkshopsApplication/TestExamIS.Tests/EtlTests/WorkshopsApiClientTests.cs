using System.Net;
using System.Text;
using System.Text.Json;
using Domain.Config;
using Domain.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Service.Implementation;
using Service.Interface;
using TestExamIS.Tests.Utils;

namespace TestExamIS.Tests.EtlTests;

[Collection("Test Suite")]
public class WorkshopsApiClientTests : LoggedTestBase
{
    private readonly Mock<HttpMessageHandler> _handler = new();
    private readonly IWorkshopsApiClient<ExternalWorkshopsDto> _client;
    private Uri? _capturedUri;
    private string? _capturedApiKey;

    public WorkshopsApiClientTests(GlobalTestFixture fixture) : base(fixture)
    {
        _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
            {
                _capturedUri = req.RequestUri;
                _capturedApiKey = req.Headers.TryGetValues("X-Api-Key", out var vals)
                    ? vals.First() : null;
            })
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(EmptyFeedJson(), Encoding.UTF8, "application/json")
            });

        var sc = new ServiceCollection();
        sc.Configure<WorkshopsApiSettings>(opts =>
        {
            opts.BaseAddress = "https://integriranisistemi.finki.ukim.mk/";
            opts.ApiKey = "kP7vXm2RwQs9TnY4LjB6HcDfE8gA1uZo";
        });

        sc.AddHttpClient<IWorkshopsApiClient<ExternalWorkshopsDto>, WorkshopsApiClient>(
                (sp, http) =>
                {
                    var settings = sp.GetRequiredService<IOptions<WorkshopsApiSettings>>().Value;
                    http.BaseAddress = new Uri(settings.BaseAddress);
                    http.DefaultRequestHeaders.Add("X-Api-Key", settings.ApiKey);
                })
            .ConfigurePrimaryHttpMessageHandler(() => _handler.Object);

        _client = sc.BuildServiceProvider()
            .GetRequiredService<IWorkshopsApiClient<ExternalWorkshopsDto>>();
    }

    [LoggedFact(Category = "ApiClient", Points = 5)]
    public async Task ApiClient_ShouldCallCorrectEndpoint()
    {
        await RunTestAsync(async () =>
        {
            await _client.GetAllWorkshopsModifiedSinceAsync(DateTime.MinValue);
            Assert.NotNull(_capturedUri);
            Assert.Contains("/api/external/workshops", _capturedUri!.PathAndQuery);
        });
    }

    [LoggedFact(Category = "ApiClient", Points = 5)]
    public async Task ApiClient_ShouldPassModifiedSinceDateAsQueryParam()
    {
        await RunTestAsync(async () =>
        {
            var date = new DateTime(2026, 3, 15, 10, 0, 0, DateTimeKind.Utc);
            await _client.GetAllWorkshopsModifiedSinceAsync(date);
            Assert.NotNull(_capturedUri);
            Assert.Contains("modifiedSince", _capturedUri!.Query);
        });
    }

    [LoggedFact(Category = "ApiClient", Points = 5)]
    public async Task ApiClient_ShouldIncludeApiKeyHeader()
    {
        await RunTestAsync(async () =>
        {
            await _client.GetAllWorkshopsModifiedSinceAsync(DateTime.MinValue);
            Assert.Equal("kP7vXm2RwQs9TnY4LjB6HcDfE8gA1uZo", _capturedApiKey);
        });
    }

    [LoggedFact(Category = "ApiClient", Points = 5)]
    public async Task ApiClient_ShouldDeserializeResponse()
    {
        await RunTestAsync(async () =>
        {
            var feedJson = JsonSerializer.Serialize(new
            {
                items = new[]
                {
                    new { externalId = "EXT-001", venueName = "Hall-A", title = "Test Workshop",
                          startTime = DateTime.UtcNow.AddDays(1),
                          endTime = DateTime.UtcNow.AddDays(1).AddHours(1),
                          lastModifiedUtc = DateTime.UtcNow, status = "Scheduled" }
                },
                page = 1, pageSize = 20, totalCount = 1, totalPages = 1, hasNextPage = false
            });

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(feedJson, Encoding.UTF8, "application/json")
                });

            var result = await _client.GetAllWorkshopsModifiedSinceAsync(DateTime.MinValue);
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.Single(result.Items);
            Assert.Equal("EXT-001", result.Items[0].ExternalId);
        });
    }

    private static string EmptyFeedJson() =>
        JsonSerializer.Serialize(new
        {
            items = Array.Empty<object>(),
            page = 1, pageSize = 20, totalCount = 0, totalPages = 0, hasNextPage = false
        });
}
