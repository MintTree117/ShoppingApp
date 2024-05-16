namespace ShopApplication.Types.Identity;

public sealed class Login
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}