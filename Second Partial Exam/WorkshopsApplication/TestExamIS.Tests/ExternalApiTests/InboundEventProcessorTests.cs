using System.Text.Json;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Interface;
using TestExamIS.Tests.Utils;

namespace TestExamIS.Tests.ExternalApiTests;

[Collection("Test Suite")]
public class InboundEventProcessorTests : LoggedTestBase, IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private const string DevApiKey = "Tk9wXm2RwQs4LnY7HjB3FcDfE5gA8uZo";

    public InboundEventProcessorTests(WebApplicationFactory<Program> factory, GlobalTestFixture fixture) : base(fixture)
    {
        _factory = factory
            .WithTestDatabase()
            .WithWebHostBuilder(b => b.ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ApiKeySettings:ApiKey"] = DevApiKey
                })));
    }

    public async Task InitializeAsync() => await TestDatabaseHelper.ResetDatabaseAsync(_factory.Services);
    public Task DisposeAsync() => Task.CompletedTask;

    private static string MakePayload(Guid workshopId, string userId, Guid venueId, string? notes = null) =>
        JsonSerializer.Serialize(new { workshopId, userId, venueId, notes });

    private async Task<InboundEventEntry> CreatePendingEntry(Guid workshopId, string userId, Guid venueId)
    {
        var entry = new InboundEventEntry
        {
            Id = Guid.NewGuid(),
            RawPayload = MakePayload(workshopId, userId, venueId),
            Status = InboundEventStatus.Pending,
            ReceivedAt = DateTime.UtcNow
        };
        return await TestDatabaseHelper.CreateEntity(_factory.Services, entry);
    }

    [LoggedFact(Category = "InboundEvent", Points = 8)]
    public async Task ProcessEventEntry_ValidPayload_ShouldCreateEnrollment()
    {
        await RunTestAsync(async () =>
        {
            var workshop = await TestDatabaseHelper.GetFirst<Workshop>(_factory.Services);
            var venue = await TestDatabaseHelper.GetFirst<Venue>(_factory.Services);
            var entry = await CreatePendingEntry(workshop.Id, "test-user-1", venue.Id);

            var countBefore = await TestDatabaseHelper.GetCount<Enrollment>(_factory.Services);
            using var scope = _factory.Services.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IInboundEventEntryProcessor>();
            var enrollment = await processor.ProcessEventEntry(entry);
            var countAfter = await TestDatabaseHelper.GetCount<Enrollment>(_factory.Services);

            Assert.NotNull(enrollment);
            Assert.Equal(countBefore + 1, countAfter);
        });
    }

    [LoggedFact(Category = "InboundEvent", Points = 5)]
    public async Task ProcessEventEntry_ValidPayload_ShouldSetEntryToCompleted()
    {
        await RunTestAsync(async () =>
        {
            var workshop = await TestDatabaseHelper.GetFirst<Workshop>(_factory.Services);
            var venue = await TestDatabaseHelper.GetFirst<Venue>(_factory.Services);
            var entry = await CreatePendingEntry(workshop.Id, "test-user-2", venue.Id);

            using var scope = _factory.Services.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IInboundEventEntryProcessor>();
            var enrollment = await processor.ProcessEventEntry(entry);

            var updatedEntry = await TestDatabaseHelper.GetFirstWhere<InboundEventEntry>(
                _factory.Services, e => e.Id == entry.Id);

            Assert.NotNull(updatedEntry);
            Assert.Equal(InboundEventStatus.Completed, updatedEntry!.Status);
            Assert.NotNull(updatedEntry.ProcessedAt);
            Assert.Equal(enrollment.Id, updatedEntry.EnrollmentId);
        });
    }

    [LoggedFact(Category = "InboundEvent", Points = 5)]
    public async Task ProcessEventEntry_InvalidPayload_ShouldSetEntryToFailed()
    {
        await RunTestAsync(async () =>
        {
            var entry = new InboundEventEntry
            {
                Id = Guid.NewGuid(),
                RawPayload = "{ this is not valid json {{ ",
                Status = InboundEventStatus.Pending,
                ReceivedAt = DateTime.UtcNow
            };
            await TestDatabaseHelper.CreateEntity(_factory.Services, entry);

            var countBefore = await TestDatabaseHelper.GetCount<Enrollment>(_factory.Services);
            using var scope = _factory.Services.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IInboundEventEntryProcessor>();
            await processor.ProcessEventEntry(entry);
            var countAfter = await TestDatabaseHelper.GetCount<Enrollment>(_factory.Services);
            Assert.Equal(countBefore, countAfter);

            var updatedEntry = await TestDatabaseHelper.GetFirstWhere<InboundEventEntry>(
                _factory.Services, e => e.Id == entry.Id);
            Assert.NotNull(updatedEntry);
            Assert.Equal(InboundEventStatus.Failed, updatedEntry!.Status);
            Assert.NotNull(updatedEntry.ErrorMessage);
        });
    }

    [LoggedFact(Category = "InboundEvent", Points = 8)]
    public async Task ProcessPendingEvents_ShouldProcessAllPendingEntries()
    {
        await RunTestAsync(async () =>
        {
            var workshop = await TestDatabaseHelper.GetFirst<Workshop>(_factory.Services);
            var venue = await TestDatabaseHelper.GetFirst<Venue>(_factory.Services);

            await CreatePendingEntry(workshop.Id, "test-user-5", venue.Id);
            await CreatePendingEntry(workshop.Id, "test-user-6", venue.Id);
            await CreatePendingEntry(workshop.Id, "test-user-7", venue.Id);

            var countBefore = await TestDatabaseHelper.GetCount<Enrollment>(_factory.Services);
            using var scope = _factory.Services.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IInboundEventEntryProcessor>();
            await processor.ProcessPendingEventsAsync();
            var countAfter = await TestDatabaseHelper.GetCount<Enrollment>(_factory.Services);
            Assert.Equal(countBefore + 3, countAfter);

            var stillPending = await TestDatabaseHelper.GetAllWhere<InboundEventEntry>(
                _factory.Services, e => e.Status == InboundEventStatus.Pending);
            Assert.Empty(stillPending);
        });
    }

    [LoggedFact(Category = "InboundEvent", Points = 5)]
    public async Task ProcessPendingEvents_ShouldIgnoreNonPendingEntries()
    {
        await RunTestAsync(async () =>
        {
            var workshop = await TestDatabaseHelper.GetFirst<Workshop>(_factory.Services);
            var venue = await TestDatabaseHelper.GetFirst<Venue>(_factory.Services);

            var alreadyCompleted = new InboundEventEntry
            {
                Id = Guid.NewGuid(),
                RawPayload = MakePayload(workshop.Id, "test-user-8", venue.Id),
                Status = InboundEventStatus.Completed,
                ReceivedAt = DateTime.UtcNow.AddMinutes(-5),
                ProcessedAt = DateTime.UtcNow.AddMinutes(-4)
            };
            await TestDatabaseHelper.CreateEntity(_factory.Services, alreadyCompleted);
            await CreatePendingEntry(workshop.Id, "test-user-9", venue.Id);

            var countBefore = await TestDatabaseHelper.GetCount<Enrollment>(_factory.Services);
            using var scope = _factory.Services.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IInboundEventEntryProcessor>();
            await processor.ProcessPendingEventsAsync();
            var countAfter = await TestDatabaseHelper.GetCount<Enrollment>(_factory.Services);
            Assert.Equal(countBefore + 1, countAfter);
        });
    }
}
