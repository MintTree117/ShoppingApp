namespace Shop.Utilities;

internal static class Extentions
{
    
    
    internal static string GetOrThrow( this IConfiguration configuration, string section ) =>
        configuration[section] ?? throw new Exception( $"Failed to get {section} from IConfiguration." );
    internal static Exception Exception( this IConfiguration configuration, string section ) =>
        new( $"Failed to get section {section} from IConfiguration." );
    
    internal static string GetIdentityApi( string api, IConfiguration configuration ) =>
        configuration[$"Identity:Apis:{api}"] ?? $"Failed to get identity api url {api} from configuration.";
    internal static string GetIdentityPage( string page, IConfiguration configuration ) =>
        configuration[$"Identity:Pages:{page}"] ?? $"Failed to get identity page url {page} from configuration.";

    internal static string ConstructReturnUrl( string url, string? returnUrl ) =>
        $"{url}?{Consts.ParamReturnUrl}={returnUrl ?? "/"}";
}