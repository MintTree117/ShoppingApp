namespace ShopWeb.Utilities;

public static class Utils
{
    public static string GetIdentityApi( string api, IConfiguration configuration ) =>
        configuration[$"Identity:Apis:{api}"] ?? $"Failed to get identity api url {api} from configuration.";
    public static string GetIdentityPage( string page, IConfiguration configuration ) =>
        configuration[$"Identity:Pages:{page}"] ?? $"Failed to get identity page url {page} from configuration.";
}