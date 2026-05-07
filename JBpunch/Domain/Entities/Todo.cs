namespace JBpunch.Domain.Entities;

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly? DueBy { get; set; }
    public bool IsComplete { get; set; }
}
