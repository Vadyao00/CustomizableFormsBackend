namespace CustomizableForms.Domain.DTOs;

public record FormDto
{
    public Guid Id { get; init; }
    public DateTime SubmittedAt { get; init; }
    public UserDto User { get; init; }
    public TemplateDto Template { get; init; }
    public List<AnswerDto> Answers { get; init; }
}