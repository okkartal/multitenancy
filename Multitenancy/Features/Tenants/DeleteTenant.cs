using Carter;
using Microsoft.AspNetCore.Mvc;
using Multitenancy.Database;

namespace Multitenancy.Features.Tenants;

public class DeleteTenantEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapDelete("/api/tenants/{id}", Handle);
	}

	private static async Task<IResult> Handle(
		[FromRoute] Guid id,
		ApplicationDbContext context,
		CancellationToken cancellationToken)
	{
		var tenant = await context.Tenants.FindAsync([id], cancellationToken);
		if (tenant is null)
		{
			return Results.NotFound();
		}

		context.Tenants.Remove(tenant);
		await context.SaveChangesAsync(cancellationToken);

		return Results.NoContent();
	}
}

