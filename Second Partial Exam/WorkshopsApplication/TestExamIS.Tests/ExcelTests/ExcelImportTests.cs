using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using ClosedXML.Excel;
using Domain.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Service.Interface;
using TestExamIS.Tests.Utils;

namespace TestExamIS.Tests.ExcelTests;

[Collection("Test Suite")]
public class ExcelImportTests : LoggedTestBase, IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ExcelImportTests(WebApplicationFactory<Program> factory, GlobalTestFixture fixture) : base(fixture)
    {
        _factory = factory.WithTestDatabase().WithTestAuth();
        _client = _factory.CreateAuthenticatedClient();
        _client.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task InitializeAsync() => await TestDatabaseHelper.ResetDatabaseAsync(_factory.Services);
    public Task DisposeAsync() => Task.CompletedTask;

    private byte[] CreateTestExcel(params (string title, DateTime start, DateTime end, string venue)[] rows)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Workshops");
        ws.Cell(1, 1).Value = "Title";
        ws.Cell(1, 2).Value = "StartDate";
        ws.Cell(1, 3).Value = "EndDate";
        ws.Cell(1, 4).Value = "Venue";
        ws.Cell(1, 5).Value = "Description";

        for (int i = 0; i < rows.Length; i++)
        {
            ws.Cell(i + 2, 1).Value = rows[i].title;
            ws.Cell(i + 2, 2).Value = rows[i].start;
            ws.Cell(i + 2, 3).Value = rows[i].end;
            ws.Cell(i + 2, 4).Value = rows[i].venue;
            ws.Cell(i + 2, 5).Value = "Test description";
        }

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    private MultipartFormDataContent CreateFileContent(byte[] excelBytes, string fileName = "test.xlsx")
    {
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(excelBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        content.Add(fileContent, "file", fileName);
        return content;
    }

    [LoggedFact(Category = "ExcelImport", Points = 5)]
    public async Task IExcelImportService_ShouldBeRegisteredInDI()
    {
        await RunTestAsync(async () =>
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IExcelImportService>();
            Assert.NotNull(service);
            await Task.CompletedTask;
        });
    }

    [LoggedFact(Category = "ExcelImport", Points = 8)]
    public async Task ImportWorkshops_ValidFile_ShouldCreateWorkshops()
    {
        await RunTestAsync(async () =>
        {
            var venue = await TestDatabaseHelper.GetFirst<Venue>(_factory.Services);
            var now = DateTime.UtcNow.AddDays(60);
            var excelBytes = CreateTestExcel(
                ("Imported WS 1", now, now.AddHours(2), venue.Name),
                ("Imported WS 2", now.AddDays(1), now.AddDays(1).AddHours(3), venue.Name));

            var countBefore = await TestDatabaseHelper.GetCount<Workshop>(_factory.Services);
            var response = await _client.PostAsync("/api/Import/workshops", CreateFileContent(excelBytes));
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.GetProperty("success").GetBoolean());
            Assert.Equal(2, doc.RootElement.GetProperty("createdCount").GetInt32());

            var countAfter = await TestDatabaseHelper.GetCount<Workshop>(_factory.Services);
            Assert.Equal(countBefore + 2, countAfter);
        });
    }

    [LoggedFact(Category = "ExcelImport", Points = 5)]
    public async Task ImportWorkshops_InvalidVenue_ShouldReturnErrors()
    {
        await RunTestAsync(async () =>
        {
            var now = DateTime.UtcNow.AddDays(60);
            var excelBytes = CreateTestExcel(
                ("WS with bad venue", now, now.AddHours(2), "NonExistentVenue999"));

            var response = await _client.PostAsync("/api/Import/workshops", CreateFileContent(excelBytes));
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.False(doc.RootElement.GetProperty("success").GetBoolean());
            Assert.True(doc.RootElement.GetProperty("errorCount").GetInt32() > 0);
        });
    }

    [LoggedFact(Category = "ExcelImport", Points = 5)]
    public async Task ImportWorkshops_EndBeforeStart_ShouldReturnErrors()
    {
        await RunTestAsync(async () =>
        {
            var venue = await TestDatabaseHelper.GetFirst<Venue>(_factory.Services);
            var now = DateTime.UtcNow.AddDays(60);
            var excelBytes = CreateTestExcel(
                ("Bad date WS", now.AddHours(5), now, venue.Name));

            var response = await _client.PostAsync("/api/Import/workshops", CreateFileContent(excelBytes));
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.False(doc.RootElement.GetProperty("success").GetBoolean());
        });
    }

    [LoggedFact(Category = "ExcelImport", Points = 5)]
    public async Task ImportWorkshops_MissingRequiredColumn_ShouldReturnErrors()
    {
        await RunTestAsync(async () =>
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Bad");
            ws.Cell(1, 1).Value = "Title";
            ws.Cell(1, 2).Value = "SomeRandomColumn";
            ws.Cell(2, 1).Value = "Test";
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);

            var response = await _client.PostAsync("/api/Import/workshops", CreateFileContent(ms.ToArray()));
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.False(doc.RootElement.GetProperty("success").GetBoolean());
        });
    }

    [LoggedFact(Category = "ExcelImport", Points = 3)]
    public async Task ImportWorkshops_NonXlsxFile_ShouldReturnBadRequest()
    {
        await RunTestAsync(async () =>
        {
            var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            content.Add(fileContent, "file", "test.txt");

            var response = await _client.PostAsync("/api/Import/workshops", content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        });
    }
}
