using Domain.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Service.Interface;
using TestExamIS.Tests.Utils;
using ClosedXML.Excel;

namespace TestExamIS.Tests.ExcelTests;

[Collection("Test Suite")]
public class ExcelExportTests : LoggedTestBase, IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ExcelExportTests(WebApplicationFactory<Program> factory, GlobalTestFixture fixture) : base(fixture)
    {
        _factory = factory.WithTestDatabase().WithTestAuth();
        _client = _factory.CreateAuthenticatedClient();
        _client.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task InitializeAsync() => await TestDatabaseHelper.ResetDatabaseAsync(_factory.Services);
    public Task DisposeAsync() => Task.CompletedTask;

    [LoggedFact(Category = "ExcelExport", Points = 5)]
    public async Task IExcelExportService_ShouldBeRegisteredInDI()
    {
        await RunTestAsync(async () =>
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IExcelExportService>();
            Assert.NotNull(service);
            await Task.CompletedTask;
        });
    }

    [LoggedFact(Category = "ExcelExport", Points = 8)]
    public async Task ExportEnrollments_ShouldReturnValidExcelBytes()
    {
        await RunTestAsync(async () =>
        {
            var workshop = await TestDatabaseHelper.GetFirst<Workshop>(_factory.Services);

            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IExcelExportService>();
            var bytes = await service.ExportEnrollmentsToExcelAsync(workshop.Id);

            Assert.NotNull(bytes);
            Assert.True(bytes.Length > 0, "Export should return non-empty byte array");

            using var ms = new MemoryStream(bytes);
            using var workbook = new XLWorkbook(ms);
            var ws = workbook.Worksheets.First();
            Assert.Equal("Enrollments", ws.Name);
        });
    }

    [LoggedFact(Category = "ExcelExport", Points = 5)]
    public async Task ExportEnrollments_ShouldHaveCorrectHeaders()
    {
        await RunTestAsync(async () =>
        {
            var workshop = await TestDatabaseHelper.GetFirst<Workshop>(_factory.Services);

            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IExcelExportService>();
            var bytes = await service.ExportEnrollmentsToExcelAsync(workshop.Id);

            using var ms = new MemoryStream(bytes);
            using var workbook = new XLWorkbook(ms);
            var ws = workbook.Worksheets.First();

            Assert.Equal("Enrollment ID", ws.Cell(1, 1).GetString());
            Assert.Equal("User", ws.Cell(1, 2).GetString());
            Assert.Equal("Status", ws.Cell(1, 3).GetString());
        });
    }

    [LoggedFact(Category = "ExcelExport", Points = 5)]
    public async Task ExportEnrollments_HeaderShouldBeBoldStyled()
    {
        await RunTestAsync(async () =>
        {
            var workshop = await TestDatabaseHelper.GetFirst<Workshop>(_factory.Services);

            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IExcelExportService>();
            var bytes = await service.ExportEnrollmentsToExcelAsync(workshop.Id);

            using var ms = new MemoryStream(bytes);
            using var workbook = new XLWorkbook(ms);
            var ws = workbook.Worksheets.First();

            Assert.True(ws.Cell(1, 1).Style.Font.Bold, "Header row should be bold");
        });
    }

    [LoggedFact(Category = "ExcelExport", Points = 5)]
    public async Task ExportEndpoint_ShouldReturnXlsxFile()
    {
        await RunTestAsync(async () =>
        {
            var workshop = await TestDatabaseHelper.GetFirst<Workshop>(_factory.Services);
            var response = await _client.GetAsync($"/api/Export/enrollments/{workshop.Id}");
            response.EnsureSuccessStatusCode();

            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                response.Content.Headers.ContentType?.MediaType);

            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0);
        });
    }
}
