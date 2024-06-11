namespace Shop.Infrastructure.Authentication.Types;

public readonly record struct AuthRefreshEventArgs(
    string? AccessToken )
{
    public static AuthRefreshEventArgs With( string? access ) =>
        new( access );
    public static AuthRefreshEventArgs None() =>
        new( null );
}