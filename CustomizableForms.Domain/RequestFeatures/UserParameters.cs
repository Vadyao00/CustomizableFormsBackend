namespace CustomizableForms.Domain.RequestFeatures;

public class UserParameters : RequestParameters
{
    public UserParameters() => OrderBy = "Name";
}