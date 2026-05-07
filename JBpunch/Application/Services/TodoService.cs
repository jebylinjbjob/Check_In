using JBpunch.Application.Abstractions;
using JBpunch.Application.Contracts;
using JBpunch.Domain.Entities;

namespace JBpunch.Application.Services;

public class TodoService(ITodoRepository repository) : ITodoService
{
    public TodoDto[] GetAll() => repository.GetAll().Select(ToDto).ToArray();

    public TodoDto? GetById(int id)
    {
        var todo = repository.GetById(id);
        return todo is null ? null : ToDto(todo);
    }

    public TodoDto Create(CreateTodoRequest request)
    {
        var todo = new Todo
        {
            Title = request.Title,
            DueBy = request.DueBy,
            IsComplete = false
        };

        var created = repository.Add(todo);
        return ToDto(created);
    }

    public TodoDto? Update(int id, UpdateTodoRequest request)
    {
        var todo = new Todo
        {
            Title = request.Title,
            DueBy = request.DueBy,
            IsComplete = request.IsComplete
        };

        var updated = repository.Update(id, todo);
        return updated is null ? null : ToDto(updated);
    }

    public bool Delete(int id) => repository.Delete(id);

    private static TodoDto ToDto(Todo todo) => new(todo.Id, todo.Title, todo.DueBy, todo.IsComplete);
}
