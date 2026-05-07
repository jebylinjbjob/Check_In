using JBpunch.Application.Contracts;

namespace JBpunch.Application.Services;

public interface ITodoService
{
    TodoDto[] GetAll();
    TodoDto? GetById(int id);
    TodoDto Create(CreateTodoRequest request);
    TodoDto? Update(int id, UpdateTodoRequest request);
    bool Delete(int id);
}
