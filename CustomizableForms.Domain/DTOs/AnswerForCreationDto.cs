using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.DTOs;

public record AnswerForCreationDto
{
    [Required(ErrorMessage = "Question ID is required")]
    public Guid QuestionId { get; init; }
    
    public string? StringValue { get; init; }
    
    public int? IntegerValue { get; init; }
    
    public bool? BooleanValue { get; init; }
}