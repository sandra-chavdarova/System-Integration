using System.ComponentModel.DataAnnotations;

namespace ExamsApplication.Domain.Common;

public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}