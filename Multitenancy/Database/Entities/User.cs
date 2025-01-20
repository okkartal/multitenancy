namespace Multitenancy.Database.Entities;

public class User : IAuditableEntity, ITenantEntity
{
    public Guid Id { get; set; }

    public required string Email { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public Guid? TenantId { get; set; }
}
