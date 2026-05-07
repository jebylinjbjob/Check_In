using HotChocolate;

using JBpunch.Application.Contracts;
using JBpunch.Application.Services;

namespace JBpunch.Presentation.GraphQL;

public class TodoQuery
{
    public TodoDto[] GetTodos([Service] ITodoService service) => service.GetAll();

    public TodoDto? GetTodoById(int id, [Service] ITodoService service) => service.GetById(id);
}
