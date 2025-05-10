using System.Text.Json.Serialization;

namespace CustomizableForms.Domain.DTOs;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("instance_url")]
    public string InstanceUrl { get; set; }
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    
    [JsonPropertyName("issued_at")]
    public string IssuedAt { get; set; }
    
    [JsonPropertyName("signature")]
    public string Signature { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; } = 7200;
}