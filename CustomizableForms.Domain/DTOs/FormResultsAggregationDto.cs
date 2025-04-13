namespace CustomizableForms.Domain.DTOs;

public record FormResultsAggregationDto
{
    public Guid TemplateId { get; init; }
    public string TemplateTitle { get; init; }
    public int TotalResponses { get; init; }
    public List<QuestionResultDto> QuestionResults { get; init; }
}