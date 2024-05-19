using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ShopWeb.Utilities;

internal sealed class WebConfig( IConfiguration config )
{
    internal readonly PasswordCriteria PasswordRules = GetPasswordRules( config );
    internal readonly Jwt JwtRules = GetJwtRules( config );
    
    static PasswordCriteria GetPasswordRules( IConfiguration configuration ) =>
        configuration.GetSection( "Identity:Validation:PasswordCriteria" ).Get<PasswordCriteria>() ??
        throw configuration.Exception( nameof( PasswordCriteria ) );
    static Jwt GetJwtRules( IConfiguration config ) => new() {
        Key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( config.GetOrThrow( "Identity:Jwt:Key" ) ) ),
        Audience = config.GetOrThrow( "Identity:Jwt:Audience" ),
        Issuer = config.GetOrThrow( "Identity:Jwt:Issuer" ),
        AccessLifetime = TimeSpan.Parse( config.GetOrThrow( "Identity:Jwt:AccessLifetime" ) ),
        RefreshLifetime = TimeSpan.Parse( config.GetOrThrow( "Identity:Jwt:RefreshLifetime" ) )
    };

    internal sealed class PasswordCriteria
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireSpecial { get; set; }
        public string Specials { get; set; } = string.Empty;
    }

    internal sealed class Jwt
    {
        public SymmetricSecurityKey Key { get; set; } = null!;
        public string Audience { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public TimeSpan AccessLifetime { get; set; }
        public TimeSpan RefreshLifetime { get; set; }
    }
}