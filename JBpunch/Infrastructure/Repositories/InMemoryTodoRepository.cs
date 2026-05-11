using JBpunch.Application.Abstractions;
using JBpunch.Domain.Entities;

namespace JBpunch.Infrastructure.Repositories;

public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<Todo> _todos =
    [
        new() { Id = 1, Title = "Walk the dog" },
        new()
        {
            Id = 2,
            Title = "Do the dishes",
            DueBy = DateOnly.FromDateTime(DateTime.Now),
        },
        new()
        {
            Id = 3,
            Title = "Do the laundry",
            DueBy = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        },
        new() { Id = 4, Title = "Clean the bathroom" },
        new()
        {
            Id = 5,
            Title = "Clean the car",
            DueBy = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
        },
    ];

    private int _nextId = 6;

    public IReadOnlyList<Todo> GetAll() => _todos;

    public Todo? GetById(int id) => _todos.FirstOrDefault(t => t.Id == id);

    public Todo Add(Todo todo)
    {
        todo.Id = _nextId++;
        _todos.Add(todo);
        return todo;
    }

    public Todo? Update(int id, Todo todo)
    {
        var existing = GetById(id);
        if (existing is null)
        {
            return null;
        }

        existing.Title = todo.Title;
        existing.DueBy = todo.DueBy;
        existing.IsComplete = todo.IsComplete;
        return existing;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing is null)
        {
            return false;
        }

        _todos.Remove(existing);
        return true;
    }
}
