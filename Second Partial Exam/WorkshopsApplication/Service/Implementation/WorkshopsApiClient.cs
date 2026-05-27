// TODO: Implement WorkshopsApiClient
// - Implement IWorkshopsApiClient<ExternalWorkshopsDto>
// - Inject HttpClient (via constructor for typed client)
// - Call GET /api/external/workshops?modifiedSince={date}
// - Deserialize response to ExternalWorkshopsDto

using System.Net.Http.Json;
using Domain.Config;
using Domain.Dto;
using Microsoft.Extensions.Options;
using Service.Interface;

namespace Service.Implementation;

public class WorkshopsApiClient : IWorkshopsApiClient<ExternalWorkshopsDto>
{
    private readonly HttpClient _httpClient;
    private readonly WorkshopsApiSettings _settings;

    public WorkshopsApiClient(HttpClient httpClient, IOptions<WorkshopsApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<ExternalWorkshopsDto> GetAllWorkshopsModifiedSinceAsync(DateTime dateLastModified)
    {
        var apiKey = _settings.ApiKey;
        var url = $"/api/external/workshops?modifiedSince={dateLastModified}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ExternalWorkshopsDto>();
    }
}