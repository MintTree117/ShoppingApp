namespace Shop.Types.Common.ValueTypes;

public sealed class Contact
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
} 