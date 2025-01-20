namespace Multitenancy.Database.Entities;

public class Author : IAuditableEntity, ITenantEntity
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public List<Book> Books { get; set; } = [];

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public Guid? TenantId { get; set; }
}
