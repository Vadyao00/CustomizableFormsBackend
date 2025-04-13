using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.DTOs;

public record AnswerForUpdateDto
{
    [Required(ErrorMessage = "Answer ID is required")]
    public Guid Id { get; init; }
    
    public string? StringValue { get; init; }
    
    public int? IntegerValue { get; init; }
    
    public bool? BooleanValue { get; init; }
}