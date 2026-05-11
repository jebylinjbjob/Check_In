using System.Text.Json.Serialization;
using JBpunch.Application.Abstractions;
using JBpunch.Application.Contracts;
using JBpunch.Application.Services;
using JBpunch.Infrastructure.Repositories;
using JBpunch.Presentation.Endpoints;
using JBpunch.Presentation.GraphQL;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build()
    )
    .WriteTo.Console()
    .WriteTo.File("logs/jbpunch-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.UseSerilog();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.Configure<Microsoft.AspNetCore.Routing.RouteOptions>(options =>
{
    options.SetParameterPolicy<Microsoft.AspNetCore.Routing.Constraints.RegexInlineRouteConstraint>(
        "regex"
    );
});

builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddGraphQLServer().AddQueryType<TodoQuery>().AddMutationType<TodoMutation>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapTodoEndpoints();
app.MapGraphQL("/graphql");

app.Run();

[JsonSerializable(typeof(TodoDto[]))]
[JsonSerializable(typeof(TodoDto))]
[JsonSerializable(typeof(CreateTodoRequest))]
[JsonSerializable(typeof(UpdateTodoRequest))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
