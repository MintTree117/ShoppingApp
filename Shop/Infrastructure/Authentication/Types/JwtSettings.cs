using Microsoft.IdentityModel.Tokens;

namespace Shop.Infrastructure.Authentication.Types;

public sealed class JwtSettings
{
    public SymmetricSecurityKey Key { get; set; } = null!;
    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public bool ValidateAudience { get; set; } = false;
    public bool ValidateIssuer { get; set; } = false;
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public TimeSpan AccessLifetime { get; set; }
    public TimeSpan RefreshLifetime { get; set; }
}