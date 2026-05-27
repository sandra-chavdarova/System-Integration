namespace Domain.Dto;

public class EnrollmentRequestDto
{
    public Guid WorkshopId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid VenueId { get; set; }
    public string? Notes { get; set; }
}
