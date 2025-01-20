using Microsoft.EntityFrameworkCore;
using Multitenancy.Database.Entities;
using Multitenancy.Database.Mapping;
using Multitenancy.Tenants;

namespace Multitenancy.Database;

public class ApplicationDbContext(
	DbContextOptions<ApplicationDbContext> options,
	ITenantProvider tenantProvider)
	: DbContext(options)
{
	public ITenantProvider TenantProvider => tenantProvider;

    public DbSet<Author> Authors { get; set; } = default!;
    public DbSet<Book> Books { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Tenant> Tenants { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
	    modelBuilder.Entity<User>()
		    .HasQueryFilter(x => x.TenantId.Equals(TenantProvider.GetCurrentTenantInfo().TenantId));

	    modelBuilder.Entity<Author>()
		    .HasQueryFilter(x => x.TenantId.Equals(TenantProvider.GetCurrentTenantInfo().TenantId));

	    modelBuilder.Entity<Book>()
		    .HasQueryFilter(x => x.TenantId.Equals(TenantProvider.GetCurrentTenantInfo().TenantId));

        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("multitenancy");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookConfiguration).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
	    var tenantInfo = TenantProvider.GetCurrentTenantInfo();

	    var modifiedTenantEntries = ChangeTracker.Entries<ITenantEntity>()
		    .Where(x => x.State is EntityState.Added or EntityState.Modified);

	    foreach (var entry in modifiedTenantEntries)
	    {
		    entry.Entity.TenantId = tenantInfo.TenantId
		        ?? throw new InvalidOperationException($"Tenant id is required but was not provided for entity '{entry.Entity.GetType()}' with state '{entry.State}'");
	    }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
