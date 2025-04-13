namespace CustomizableForms.Domain.DTOs;

public record StringValueCountDto
{
    public string Value { get; init; }
    public int Count { get; init; }
}