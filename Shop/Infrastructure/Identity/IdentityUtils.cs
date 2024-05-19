namespace Shop.Features.Identity;

public static class IdentityUtils
{
    public static string GetApiUrl( string type, IConfiguration configuration ) =>
        configuration[$"{IdentityConsts.IdentityConfigSection}:{type}"] ?? $"Failed to get identity api url {type}.";
}