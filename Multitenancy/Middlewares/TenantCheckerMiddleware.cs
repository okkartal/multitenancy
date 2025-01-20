using Microsoft.AspNetCore.Mvc;

namespace Multitenancy.Middlewares;

public class TenantCheckerMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var tenantClaimValue = context.User.Claims.FirstOrDefault(x => x.Type.Equals("tenant-id"))?.Value;
		if (tenantClaimValue is null)
		{
			await next(context);
			return;
		}

		if (!context.Request.Headers.TryGetValue("X-TenantId", out var headerGuid))
		{
			await next(context);
			return;
		}

		if (tenantClaimValue.Contains(headerGuid.ToString(), StringComparison.Ordinal))
		{
			await next(context);
			return;
		}

		var problemDetails = new ProblemDetails
		{
			Status = StatusCodes.Status403Forbidden,
			Title = "Bad Request",
			Detail = "X-TenantId header contains a tenant id that a user doesn't have access to"
		};

		context.Response.StatusCode = problemDetails.Status.Value;
		context.Response.ContentType = "application/problem+json";

		await context.Response.WriteAsJsonAsync(problemDetails);
	}
}
