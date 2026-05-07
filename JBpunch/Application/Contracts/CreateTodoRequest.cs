namespace JBpunch.Application.Contracts;

public record CreateTodoRequest(string Title, DateOnly? DueBy);
