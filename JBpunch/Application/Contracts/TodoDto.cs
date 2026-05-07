namespace JBpunch.Application.Contracts;

public record TodoDto(int Id, string Title, DateOnly? DueBy, bool IsComplete);
