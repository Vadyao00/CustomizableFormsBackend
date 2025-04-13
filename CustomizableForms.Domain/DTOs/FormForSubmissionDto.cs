using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.DTOs;

public record FormForSubmissionDto
{
    [Required(ErrorMessage = "Answers are required")]
    public List<AnswerForCreationDto> Answers { get; init; }
}
