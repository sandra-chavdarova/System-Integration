namespace Domain.Dto;

public class ConsultationDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid RoomId { get; set; }
}