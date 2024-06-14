namespace Shop.Infrastructure.Authentication;

public readonly record struct SessionClaims(
    string SessionId,
    string UserId,
    string Username,
    DateTime Expiry );