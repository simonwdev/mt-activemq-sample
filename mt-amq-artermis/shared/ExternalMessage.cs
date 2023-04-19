namespace shared;

public record ExternalMessage
{
    public string? Value { get; set; }
    public bool? Fail { get; set; }
}
