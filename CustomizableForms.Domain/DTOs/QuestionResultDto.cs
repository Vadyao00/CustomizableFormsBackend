using CustomizableForms.Domain.Enums;

namespace CustomizableForms.Domain.DTOs;

public record QuestionResultDto
{
    public Guid QuestionId { get; init; }
    public string QuestionTitle { get; init; }
    public QuestionType Type { get; init; }
    
    public double? AverageValue { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    
    public List<StringValueCountDto> MostCommonValues { get; set; }
    
    public int? TrueCount { get; set; }
    public int? FalseCount { get; set; }
    public double? TruePercentage { get; set; }
}