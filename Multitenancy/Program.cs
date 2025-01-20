using System.Text;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; 
using Microsoft.IdentityModel.Tokens;
using Multitenancy.Configuration;
using Multitenancy.Database;
using Multitenancy.Middlewares;
using Multitenancy.Services;
using Multitenancy.Tenants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Postgres");

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<TenantCheckerMiddleware>();

builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddSingleton<AuditableInterceptor>();

builder.Services.AddDbContext<ApplicationDbContext>((provider, options) =>
{
	var interceptor = provider.GetRequiredService<AuditableInterceptor>();

    options.EnableSensitiveDataLogging()
	    .UseNpgsql(connectionString, npgsqlOptions =>
	    {
		    npgsqlOptions.MigrationsHistoryTable("__MyMigrationsHistory", "multitenancy");
	    })
	    .AddInterceptors(interceptor)
	    .UseSnakeCaseNamingConvention();

    // TODO: uncomment this only to allow EF Core to make conditional Global Query Filters
    // THIS severely damages performance of the application
    //options.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
});

builder.Services.AddOptions<AuthConfiguration>()
	.Bind(builder.Configuration.GetSection(nameof(AuthConfiguration)));

builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["AuthConfiguration:Issuer"],
			ValidAudience = builder.Configuration["AuthConfiguration:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthConfiguration:Key"]!))
		};
	});

builder.Services.AddCarter();

var app = builder.Build();

app.UseMiddleware<TenantCheckerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();

// Create and seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    await DatabaseSeedService.SeedAsync(dbContext);
}

await app.RunAsync();
