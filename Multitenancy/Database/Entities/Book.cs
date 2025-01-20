namespace Multitenancy.Database.Entities;

public class Book : IAuditableEntity, ITenantEntity
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public required int Year { get; set; }

    public Guid AuthorId { get; set; }

    public Author Author { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public Guid? TenantId { get; set; }
}
