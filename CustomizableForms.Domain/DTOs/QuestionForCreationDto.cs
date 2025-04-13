using System.ComponentModel.DataAnnotations;
using CustomizableForms.Domain.Enums;

namespace CustomizableForms.Domain.DTOs;

public record QuestionForCreationDto
{
    [Required(ErrorMessage = "Question title is required")]
    [MaxLength(500, ErrorMessage = "Maximum length for the Title is 500 characters")]
    public string Title { get; init; }
    
    public string Description { get; init; }
    
    public int OrderIndex { get; init; }
    
    public bool ShowInResults { get; init; } = true;
    
    [Required(ErrorMessage = "Question type is required")]
    public QuestionType Type { get; init; }
}