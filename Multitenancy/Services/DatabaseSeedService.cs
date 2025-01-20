using Microsoft.EntityFrameworkCore;
using Multitenancy.Database;
using Multitenancy.Database.Entities;

namespace Multitenancy.Services;

public static class DatabaseSeedService
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
	    await dbContext.Database.MigrateAsync();

	    if (await dbContext.Tenants.AnyAsync())
	    {
		    return;
	    }

	    var tenant = new Tenant
	    {
		    Id = Guid.NewGuid(),
		    Name = "tenant-1"
	    };

	    await dbContext.Tenants.AddAsync(tenant);
	    await dbContext.SaveChangesAsync();
    }
}
