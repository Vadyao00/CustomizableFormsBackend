namespace CustomizableForms.Domain.Entities;

public class Answer
{
    public Guid Id { get; set; }
    public string? StringValue { get; set; }
    public int? IntegerValue { get; set; }
    public bool? BooleanValue { get; set; }
    
    public Guid FormId { get; set; }
    public Form Form { get; set; }
    
    public Guid QuestionId { get; set; }
    public Question Question { get; set; }
}