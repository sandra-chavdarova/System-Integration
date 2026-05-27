using System.Threading.Channels;
using Domain.Config;
using Domain.Dto.Email;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Interface;
using TestExamIS.Tests.Utils;

namespace TestExamIS.Tests.EmailTests;

[Collection("Test Suite")]
public class EmailServiceTests : LoggedTestBase, IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;

    public EmailServiceTests(WebApplicationFactory<Program> factory, GlobalTestFixture fixture) : base(fixture)
    {
        _factory = factory.WithTestDatabase();
    }

    public async Task InitializeAsync() => await TestDatabaseHelper.ResetDatabaseAsync(_factory.Services);
    public Task DisposeAsync() => Task.CompletedTask;

    [LoggedFact(Category = "Email", Points = 5)]
    public async Task EmailSettings_ShouldBeConfigured()
    {
        await RunTestAsync(async () =>
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(typeof(Program).Assembly.Location)!)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var smtpHost = config["EmailSettings:SmtpHost"];
            Assert.NotNull(smtpHost);
            Assert.False(string.IsNullOrEmpty(smtpHost), "EmailSettings:SmtpHost must be configured");

            var fromAddress = config["EmailSettings:FromAddress"];
            Assert.NotNull(fromAddress);
            Assert.False(string.IsNullOrEmpty(fromAddress), "EmailSettings:FromAddress must be configured");
            await Task.CompletedTask;
        });
    }

    [LoggedFact(Category = "Email", Points = 5)]
    public async Task IEmailService_ShouldBeRegisteredInDI()
    {
        await RunTestAsync(async () =>
        {
            using var scope = _factory.Services.CreateScope();
            var emailService = scope.ServiceProvider.GetService<IEmailService>();
            Assert.NotNull(emailService);
            await Task.CompletedTask;
        });
    }

    [LoggedFact(Category = "Email", Points = 5)]
    public async Task IEmailQueue_ShouldBeRegisteredInDI()
    {
        await RunTestAsync(async () =>
        {
            var emailQueue = _factory.Services.GetService<IEmailQueue>();
            Assert.NotNull(emailQueue);
            await Task.CompletedTask;
        });
    }

    [LoggedFact(Category = "Email", Points = 5)]
    public async Task EmailQueue_ShouldAcceptMessages()
    {
        await RunTestAsync(async () =>
        {
            var queue = _factory.Services.GetRequiredService<IEmailQueue>();
            var message = new EmailMessage
            {
                To = "test@example.com",
                Subject = "Test",
                HtmlBody = "<p>Test</p>"
            };
            await queue.EnqueueAsync(message);

            var channel = _factory.Services.GetRequiredService<Channel<EmailMessage>>();
            Assert.True(channel.Reader.TryRead(out var read));
            Assert.Equal("test@example.com", read!.To);
        });
    }

    [LoggedFact(Category = "Email", Points = 5)]
    public async Task EmailBackgroundService_ShouldBeRegisteredAsHostedService()
    {
        await RunTestAsync(async () =>
        {
            var hostedServices = _factory.Services.GetServices<IHostedService>();
            var hasEmailBg = hostedServices
                .Any(s => s.GetType().Name.Contains("Email", StringComparison.OrdinalIgnoreCase)
                       && s.GetType().IsAssignableTo(typeof(BackgroundService)));
            Assert.True(hasEmailBg, "An EmailBackgroundService must be registered as IHostedService");
            await Task.CompletedTask;
        });
    }
}
