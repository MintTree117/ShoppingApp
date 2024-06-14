namespace Shop.Infrastructure.Authentication;

public readonly record struct SessionInfo(
    string UserId,
    string Username );