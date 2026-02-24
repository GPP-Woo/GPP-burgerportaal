using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using ODBP.Apis.Odrc;
using ODBP.Apis.Search;
using ODBP.Authentication;
using ODBP.Config;
using ODBP.Data;
using ODBP.Features;
using ODBP.Features.Beheer;
using ODBP.Features.Sitemap;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

using var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

logger.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    string GetRequiredConfig(string key)
    {
        var value = builder.Configuration[key];

        return string.IsNullOrWhiteSpace(value)
            ? throw new Exception($"Environment variable {key} is missing or empty")
            : value;
    };

    builder.Host.UseSerilog(logger);

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddHealthChecks();
    builder.Services.AddHttpClient();

    builder.Services.AddAuth(options =>
    {
        options.Authority = GetRequiredConfig("OIDC_AUTHORITY");
        options.ClientId = GetRequiredConfig("OIDC_CLIENT_ID");
        options.ClientSecret = GetRequiredConfig("OIDC_CLIENT_SECRET");
        options.AdminRole = GetRequiredConfig("OIDC_ADMIN_ROLE");
        options.NameClaimType = builder.Configuration["OIDC_NAME_CLAIM_TYPE"];
        options.RoleClaimType = builder.Configuration["OIDC_ROLE_CLAIM_TYPE"];
        options.IdClaimType = builder.Configuration["OIDC_ID_CLAIM_TYPE"];
    });

    var connStr = $"Username={builder.Configuration["POSTGRES_USER"]};Password={builder.Configuration["POSTGRES_PASSWORD"]};Host={builder.Configuration["POSTGRES_HOST"]};Database={builder.Configuration["POSTGRES_DB"]};Port={builder.Configuration["POSTGRES_PORT"]}";
    builder.Services.AddDbContext<OdbpDbContext>(opt => opt.UseNpgsql(connStr));

    builder.Services.AddSingleton<IOdrcClientFactory, OdrcClientFactory>();
    builder.Services.AddBaseUri();

    // de sitemap duurt even om te genereren. de cache gaat in als de sitemap klaar is
    // stel een crawler draait elke dag om 01:00. dan is de sitemap bv om 01:01 klaar en dan gaat de cache in
    // als de cache dan pas de volgende dag om 01:01 verloopt, krijgt de crawler de volgende dag de gecachete waarde.
    // daarom maar een uurtje minder lang cachen
    const int DefaultCacheExpiryHours = 23;

    var cacheExpiryHours = double.TryParse(builder.Configuration["SITEMAP_CACHE_DURATION_HOURS"], out var d)
        ? d
        : DefaultCacheExpiryHours;

    builder.Services.AddOutputCache(x => x.AddPolicy(OutputCachePolicies.Sitemap,
        b => b.Expire(TimeSpan.FromHours(cacheExpiryHours))));

    builder.Services.AddSingleton<ResourcesConfig>();
    builder.Services.AddSingleton<StorageConfig>();
    builder.Services.AddHttpClient<ISearchClient, SearchClient>(httpClient =>
    {
        httpClient.BaseAddress = new(GetRequiredConfig("SEARCH_BASE_URL"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", GetRequiredConfig("SEARCH_API_KEY"));
    });
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<ISimpleCache, SimpleCache>();
    builder.Services.AddScoped<MigrateResources>();

    var app = builder.Build();

    app.UseSerilogRequestLogging(x => x.Logger = logger);
    app.UseDefaultFiles();
    app.UseOdbpStaticFiles();

    if (!app.Environment.IsDevelopment())
    {
        app.UseOutputCache();
    }

    app.UseOdbpSecurityHeaders();

    app.UseCoep();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapAuthEndpoints();

    app.MapControllers();
    app.MapHealthChecks("/healthz");
    app.MapFallbackToIndexHtml();

    await using (var scope = app.Services.CreateAsyncScope())
    {
        await scope.ServiceProvider.GetRequiredService<OdbpDbContext>().Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<MigrateResources>().ExecuteAsync();
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    logger.Write(LogEventLevel.Fatal, ex, "Application terminated unexpectedly");
}

public partial class Program { }
