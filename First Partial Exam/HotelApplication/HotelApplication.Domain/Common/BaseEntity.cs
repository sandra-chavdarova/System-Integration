namespace HotelApplication.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = default!;
}
