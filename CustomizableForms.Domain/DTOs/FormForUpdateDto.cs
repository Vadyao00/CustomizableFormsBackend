using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.DTOs;

public record FormForUpdateDto
{
    [Required(ErrorMessage = "Answers are required")]
    public List<AnswerForUpdateDto> Answers { get; init; }
}
