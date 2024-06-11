namespace Shop.Infrastructure.Authentication.Types;

public readonly record struct SessionInfo(
    string UserId,
    string Username );