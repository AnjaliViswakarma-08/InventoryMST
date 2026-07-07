namespace InventoryMS.Helpers;

public sealed class ErrorResponse
{
    public bool Success { get; init; } = false;

    public string Message { get; init; } = string.Empty;

    public List<string> Errors { get; init; } = new();
}
