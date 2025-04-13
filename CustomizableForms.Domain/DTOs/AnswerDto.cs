namespace CustomizableForms.Domain.DTOs;

public record AnswerDto
{
    public Guid Id { get; init; }
    public string StringValue { get; init; }
    public int? IntegerValue { get; init; }
    public bool? BooleanValue { get; init; }
    public Guid QuestionId { get; init; }
    public QuestionDto Question { get; init; }
}