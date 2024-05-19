using System.ComponentModel.DataAnnotations;

namespace Shop.Features.Account.Types;

public class AccountModelNew
{
    [Required( ErrorMessage = "Email is required." ), EmailAddress( ErrorMessage = "Invalid email address." )]
    public string Email { get; set; } = string.Empty;

    [Required( ErrorMessage = "Username is required." )]
    public string Username { get; set; } = string.Empty;

    [Phone( ErrorMessage = "Invalid phone number." )]
    public string? Phone { get; set; } = string.Empty;
}