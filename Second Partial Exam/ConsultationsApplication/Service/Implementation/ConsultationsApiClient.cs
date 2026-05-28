using System.Net.Http.Json;
using Domain.Configuration;
using Domain.Dto;
using Domain.ExternalModels;
using Domain.Models;
using Microsoft.Extensions.Options;
using Service.Interface;

namespace Service.Implementation;

public class ConsultationsApiClient : IConsultationsApiClient<ExternalConsultationsDto>
{
    private readonly HttpClient _client;
    private readonly ConsultationsApiSettings _settings;

    public ConsultationsApiClient(HttpClient client, IOptions<ConsultationsApiSettings> settings)
    {
        _client = client;
        _settings = settings.Value;
    }

    public async Task<ExternalConsultationsDto> GetAllConsultationsModifiedSinceAsync(DateTime? lastModified)
    {
        var apiKey = _settings.ApiKey;
        var url = $"api/external/consultations?modifiedSince={lastModified}";

        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ExternalConsultationsDto>();
    }
}