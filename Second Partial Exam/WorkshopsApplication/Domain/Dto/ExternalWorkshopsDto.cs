namespace Domain.Dto;

public class ExternalWorkshopsDto
{
    public List<ExternalWorkshopDto> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
}

public class ExternalWorkshopDto
{
    public string ExternalId { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime LastModifiedUtc { get; set; }
    public string Status { get; set; } = string.Empty;
}
