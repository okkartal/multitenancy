using Carter;
using Microsoft.AspNetCore.Mvc;
using Multitenancy.Database;
using Multitenancy.Features.Tenants.Shared;

namespace Multitenancy.Features.Tenants;

public class GetTenantByIdEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/api/tenants/{id}", Handle);
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

		var response = new TenantResponse(tenant.Id, tenant.Name);
		return Results.Ok(response);
	}
}

