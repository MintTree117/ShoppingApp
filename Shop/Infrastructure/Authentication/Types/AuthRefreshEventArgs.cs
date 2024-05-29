namespace Shop.Infrastructure.Authentication.Types;

public readonly record struct AuthRefreshEventArgs(
    string? accessToken,
    string? refreshToken )
{
    public static AuthRefreshEventArgs With( string? access, string? refresh ) =>
        new( access, refresh );
    public static AuthRefreshEventArgs None() =>
        new( null, null );
}