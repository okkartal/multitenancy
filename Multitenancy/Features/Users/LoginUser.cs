using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Multitenancy.Configuration;
using Multitenancy.Database;
using Multitenancy.Database.Entities;

namespace Multitenancy.Features.Users;

public sealed record LoginUserRequest(string Email);

public class LoginUserEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("/api/users/login", Handle);
	}

	private static async Task<IResult> Handle(
		[FromBody] LoginUserRequest request,
		ApplicationDbContext context,
		IOptions<AuthConfiguration> jwtSettingsOptions,
		CancellationToken cancellationToken)
	{
		var user = await context.Users
			.IgnoreQueryFilters()
			.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

		if (user is null)
		{
			return Results.NotFound("User not found");
		}

		var token = GenerateJwtToken(user, jwtSettingsOptions.Value);
		return Results.Ok(new { Token = token });
	}

	private static string GenerateJwtToken(User user, AuthConfiguration auth)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(auth.Key));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Email),
			new Claim("use-id", user.Id.ToString()),
			new Claim("tenant-id", user.TenantId?.ToString() ?? string.Empty)
		};

		var token = new JwtSecurityToken(
			issuer: auth.Issuer,
			audience: auth.Audience,
			claims: claims,
			expires: DateTime.Now.AddMinutes(30),
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
