namespace ShopWeb.Utilities;

internal static class WebUtils
{
    internal static string GetIdentityApi( string api, IConfiguration configuration ) =>
        configuration[$"Identity:Apis:{api}"] ?? $"Failed to get identity api url {api} from configuration.";
    internal static string GetIdentityPage( string page, IConfiguration configuration ) =>
        configuration[$"Identity:Pages:{page}"] ?? $"Failed to get identity page url {page} from configuration.";

    internal static string ConstructReturnUrl( string url, string? returnUrl ) =>
        $"{url}?{WebConsts.ReturnUrl}={returnUrl ?? "/"}";
}