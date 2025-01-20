using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Multitenancy.Database;

namespace Multitenancy.Features.Tenants;

public sealed record UpdateTenantRequest(Guid Id, string Name);

public class UpdateTenantEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPut("/api/tenants/{id}", Handle);
	}

	private static async Task<IResult> Handle(
		[FromRoute] Guid id,
		[FromBody] UpdateTenantRequest request,
		ApplicationDbContext context,
		CancellationToken cancellationToken)
	{
		var tenant = await context.Tenants
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

		if (tenant is null)
		{
			return Results.NotFound();
		}

		tenant.Name = request.Name;

		await context.SaveChangesAsync(cancellationToken);

		return Results.NoContent();
	}
}
