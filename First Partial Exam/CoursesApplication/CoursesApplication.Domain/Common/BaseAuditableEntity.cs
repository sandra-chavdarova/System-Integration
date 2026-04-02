namespace CoursesApplication.Domain.Common;

public abstract class BaseAuditableEntity<TU> : BaseEntity
{
    public required string CreatedById { get; set; }
    public DateTime DateCreated { get; set; }
    
    public required string LastModifiedById { get; set; }
    public DateTime? DateLastModified { get; set; }
}
