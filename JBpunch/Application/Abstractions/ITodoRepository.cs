using JBpunch.Domain.Entities;

namespace JBpunch.Application.Abstractions;

public interface ITodoRepository
{
    IReadOnlyList<Todo> GetAll();
    Todo? GetById(int id);
    Todo Add(Todo todo);
    Todo? Update(int id, Todo todo);
    bool Delete(int id);
}
