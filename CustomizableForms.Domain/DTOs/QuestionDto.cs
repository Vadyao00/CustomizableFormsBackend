using CustomizableForms.Domain.Enums;

namespace CustomizableForms.Domain.DTOs;

public record QuestionDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public int OrderIndex { get; init; }
    public bool ShowInResults { get; init; }
    public QuestionType Type { get; init; }
}