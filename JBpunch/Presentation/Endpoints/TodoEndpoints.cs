using JBpunch.Application.Contracts;
using JBpunch.Application.Services;

using Microsoft.AspNetCore.Http.HttpResults;

namespace JBpunch.Presentation.Endpoints;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var todos = app.MapGroup("/todos");

        todos.MapGet("/", (ITodoService service) => service.GetAll())
            .WithName("GetTodos");

        todos.MapGet("/{id:int}", Results<Ok<TodoDto>, NotFound> (int id, ITodoService service) =>
            service.GetById(id) is { } todo
                ? TypedResults.Ok(todo)
                : TypedResults.NotFound())

            .WithName("GetTodoById");

        todos.MapPost("/", (CreateTodoRequest request, ITodoService service) =>
        {
            var created = service.Create(request);
            return TypedResults.Created($"/todos/{created.Id}", created);
        })
            .WithName("CreateTodo");

        todos.MapPut("/{id:int}", Results<Ok<TodoDto>, NotFound> (int id, UpdateTodoRequest request, ITodoService service) =>
            service.Update(id, request) is { } todo
                ? TypedResults.Ok(todo)
                : TypedResults.NotFound())
            .WithName("UpdateTodo");

        todos.MapDelete("/{id:int}", Results<NoContent, NotFound> (int id, ITodoService service) =>
            service.Delete(id)
                ? TypedResults.NoContent()
                : TypedResults.NotFound())
            .WithName("DeleteTodo");

        return app;
    }
}
