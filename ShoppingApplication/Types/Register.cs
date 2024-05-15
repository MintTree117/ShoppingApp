namespace ShoppingApplication.Types;

internal sealed class Register
{
    internal string Email { get; set; } = string.Empty;
    internal string Username { get; set; } = string.Empty;
    internal string Phone { get; set; } = string.Empty;
    internal string Password { get; set; } = string.Empty;
    internal string PasswordConfirm { get; set; } = string.Empty;
}