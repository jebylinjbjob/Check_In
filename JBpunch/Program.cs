using JBpunch;
using JBpunch.Application.Contracts;
using JBpunch.Application.Services;
using JBpunch.Authorization;
using JBpunch.Data;
using JBpunch.DTOs;
using JBpunch.Presentation.Endpoints;
using JBpunch.Presentation.GraphQL;
using JBpunch.Repositories;
using JBpunch.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
var allowedOrigins =
    builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://127.0.0.1:5500"];

// 允許來自前端的跨域請求
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }
    );
});

builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddOpenApi();

builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName)
);
builder.Services.AddDbContext<MyDbContext>(options =>
{
    var provider =
        builder.Configuration[$"{DatabaseOptions.SectionName}:Provider"]?.ToLowerInvariant()
        ?? "mssql";

    if (provider == "postgresql" || provider == "postgres")
    {
        var connectionString =
            builder.Configuration.GetConnectionString("PostgreSql")
            ?? throw new InvalidOperationException("Connection string 'PostgreSql' is missing.");
        options.UseNpgsql(connectionString);
    }
    else if (provider == "mssql" || provider == "sqlserver")
    {
        var connectionString =
            builder.Configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Connection string 'SqlServer' is missing.");
        options.UseSqlServer(connectionString);
    }
    else
    {
        throw new InvalidOperationException($"Unsupported database provider: {provider}");
    }
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IApplicationService<>), typeof(ApplicationService<>));
builder.Services.AddScoped<IClockDataService, ClockDataService>();
builder.Services.AddScoped<IGpsPuncheService, GpsPuncheService>();

// ABP JWT Bearer 身分驗證
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["AuthServer:Authority"];
        options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>(
            "AuthServer:RequireHttpsMetadata",
            false
        );
        options.Audience = builder.Configuration["AuthServer:Audience"];
    });

// 註冊授權相關服務
builder.Services.AddHttpClient("AuthServer");
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IRemotePermissionChecker, RemotePermissionChecker>();
builder.Services.AddSingleton<IAuthorizationHandler, AbpPermissionHandler>();

// 註冊 ABP 權限 Policy：每個權限名稱對應一個 Policy
builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy("AbpAuthenticated", policy => policy.RequireAuthenticatedUser());
foreach (var permission in JBpunchPermissions.GetAll())
{
    builder
        .Services.AddAuthorizationBuilder()
        .AddPolicy(
            permission,
            policy => policy.Requirements.Add(new AbpPermissionRequirement(permission))
        );
}

builder.Services.AddHealthChecks().AddDbContextCheck<MyDbContext>();

builder.Services.AddGraphQLServer().AddQueryType<Query>().AddMutationType<Mutation>();

var app = builder.Build();

// 啟動時自動加入測試資料
DateSeed.SeedData(app.Services);

ApplyPathBaseFromConfiguration(app);

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

// app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// 根路徑回傳服務資訊
app.MapGet(
    "/",
    async (HttpContext ctx, MyDbContext db) =>
    {
        var dbConnection = false;
        try
        {
            dbConnection = await db.Database.CanConnectAsync();
        }
        catch
        {
            dbConnection = false;
        }

        return Results.Json(
            new ServiceInfoDto(
                Service: "jbpunch",
                PathBase: ctx.Request.PathBase.Value,
                Openapi: $"{ctx.Request.PathBase}/openapi/v1.json",
                Scalar: $"{ctx.Request.PathBase}/scalar/v1",
                Graphql: $"{ctx.Request.PathBase}/graphql",
                DBConnection: dbConnection
            )
        );
    }
);

app.MapHealthChecks("/health");

// 驗證身分驗證是否正常的測試端點
app.MapGet(
        "/auth-test",
        (HttpContext ctx) =>
        {
            var user = ctx.User;
            return Results.Json(
                new AuthTestDto(
                    IsAuthenticated: user.Identity?.IsAuthenticated ?? false,
                    AuthenticationType: user.Identity?.AuthenticationType,
                    UserName: user.Identity?.Name,
                    Claims: user.Claims.Select(c => new ClaimDto(c.Type, c.Value)).ToArray()
                )
            );
        }
    )
    .RequireAuthorization();

app.MapClockDataEndpoints();
app.MapGpsPuncheEndpoints();
app.MapGraphQL();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

static void ApplyPathBaseFromConfiguration(WebApplication application)
{
    var opts =
        application
            .Configuration.GetSection(ApplicationOptions.SectionName)
            .Get<ApplicationOptions>()
        ?? new ApplicationOptions();
    var pathBase = NormalizeApplicationPathBase(opts.PathBase);
    if (pathBase.HasValue)
        application.UsePathBase(pathBase);
}

static PathString NormalizeApplicationPathBase(string? raw)
{
    if (string.IsNullOrWhiteSpace(raw))
        return PathString.Empty;

    var t = raw.Trim();
    if (t == "/")
        return PathString.Empty;

    if (!t.StartsWith('/'))
        t = "/" + t;

    t = t.TrimEnd('/');
    if (t.Length == 0 || t == "/")
        return PathString.Empty;

    return new PathString(t);
}

public partial class Program { }
