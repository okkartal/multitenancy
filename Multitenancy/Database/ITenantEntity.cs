namespace Multitenancy.Database;

public interface ITenantEntity
{
	public Guid? TenantId { get; set; }
}
