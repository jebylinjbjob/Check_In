using HotChocolate;

using JBpunch.Application.Contracts;
using JBpunch.Application.Services;

namespace JBpunch.Presentation.GraphQL;

public class TodoMutation
{
    public TodoDto CreateTodo(CreateTodoRequest input, [Service] ITodoService service) => service.Create(input);

    public TodoDto? UpdateTodo(int id, UpdateTodoRequest input, [Service] ITodoService service) =>
        service.Update(id, input);

    public bool DeleteTodo(int id, [Service] ITodoService service) => service.Delete(id);
}
