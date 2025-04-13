namespace CustomizableForms.Domain.Responses;

public abstract class ApiBaseResponse
{
    public bool Success {  get; set; }
    protected ApiBaseResponse(bool suссess) => Success = suссess;
}