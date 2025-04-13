namespace CustomizableForms.Domain.DTOs;

public record TagDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public int TemplatesCount { get; set; }
}