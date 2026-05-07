namespace JBpunch.Application.Contracts;

public record UpdateTodoRequest(string Title, DateOnly? DueBy, bool IsComplete);
