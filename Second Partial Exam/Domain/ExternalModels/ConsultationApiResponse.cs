using System.Text.Json.Serialization;

namespace Domain.ExternalModels;

public class ConsultationApiResponse
{
    [JsonPropertyName("items")] public List<ConsultationItem>? Items { get; set; }
    [JsonPropertyName("page")] public int Page { get; set; }
    [JsonPropertyName("pageSize")] public int PageSize { get; set; }
    [JsonPropertyName("totalCount")] public int TotalCount { get; set; }
    [JsonPropertyName("totalPages")] public int TotalPages { get; set; }
    [JsonPropertyName("hasNextPage")] public bool HasNextPage { get; set; }
}

public class ConsultationItem
{
    [JsonPropertyName("externalId")] public string? ExternalId { get; set; }

    [JsonPropertyName("roomName")] public string? RoomName { get; set; }

    [JsonPropertyName("startTime")] public DateTime StartTime { get; set; }
    [JsonPropertyName("endTime")] public DateTime EndTime { get; set; }
    [JsonPropertyName("lastModifiedUtc")] public DateTime LastModifiedUtc { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
}