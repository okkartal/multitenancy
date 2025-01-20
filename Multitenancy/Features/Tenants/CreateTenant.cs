using Carter;
using Microsoft.AspNetCore.Mvc;
using Multitenancy.Database;
using Multitenancy.Database.Entities;
using Multitenancy.Features.Tenants.Shared;

namespace Multitenancy.Features.Tenants;

public sealed record CreateTenantRequest(string Name);

public class CreateTenantEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("/api/tenants", Handle);
	}

	private static async Task<IResult> Handle(
		[FromBody] CreateTenantRequest request,
		ApplicationDbContext context,
		CancellationToken cancellationToken)
	{
		var tenant = new Tenant
		{
			Id = Guid.NewGuid(),
			Name = request.Name
		};

		context.Tenants.Add(tenant);
		await context.SaveChangesAsync(cancellationToken);

		var response = new TenantResponse(tenant.Id, tenant.Name);
		return Results.Created($"/api/tenants/{tenant.Id}", response);
	}
}
