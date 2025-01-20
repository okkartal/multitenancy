namespace Multitenancy.Database.Entities;

public class Tenant : IAuditableEntity
{
	public required Guid Id { get; set; }

	public required string Name { get; set; }
	public DateTime CreatedAtUtc { get; set; }

	public DateTime? UpdatedAtUtc { get; set; }
}
