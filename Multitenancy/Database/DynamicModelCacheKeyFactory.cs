using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Multitenancy.Database;

public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
{
	public object Create(DbContext context, bool designTime)
		=> context is ApplicationDbContext dynamicContext
			? (context.GetType(), dynamicContext.TenantProvider.GetCurrentTenantInfo(), designTime)
			: context.GetType();

	public object Create(DbContext context)
		=> Create(context, false);
}
