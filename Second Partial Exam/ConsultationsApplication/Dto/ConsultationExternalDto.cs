namespace Domain.Dto;

public class ConsultationExternalDto
{
    public Guid Id { get; set; }
    public string RoomName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime LastModified { get; set; }
    public string Status { get; set; }
}