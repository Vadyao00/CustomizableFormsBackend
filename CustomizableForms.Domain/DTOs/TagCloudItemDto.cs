namespace CustomizableForms.Domain.DTOs;

public record TagCloudItemDto
{
    public string Name { get; init; }
    public int Weight { get; init; }
}