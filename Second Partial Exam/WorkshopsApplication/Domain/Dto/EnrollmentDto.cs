namespace Domain.Dto;

public class EnrollmentDto
{
    public string? Notes { get; set; }
    public required string UserId { get; set; }
    public Guid VenueId { get; set; }
    public Guid WorkshopId { get; set; }
}
