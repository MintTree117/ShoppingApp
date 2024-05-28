namespace Shop.Infrastructure.Authentication;

public readonly record struct SessionModel(
    string? AccessToken,
    string? RefreshToken,
    SessionInfo? SessionInfo )
{
    public static SessionModel With( string? access, string? refresh, string? name ) =>
        new( access, refresh, name is null ? null : new SessionInfo( name ) );
}