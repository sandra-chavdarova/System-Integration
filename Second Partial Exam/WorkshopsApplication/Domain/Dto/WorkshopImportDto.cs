namespace Domain.Dto;

public class WorkshopImportDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public Guid VenueId { get; set; }
}
