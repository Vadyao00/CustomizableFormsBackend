namespace CustomizableForms.Domain.DTOs;

public record CommentDto
{
    public Guid Id { get; init; }
    public string Content { get; init; }
    public DateTime CreatedAt { get; init; }
    public UserDto User { get; init; }
}