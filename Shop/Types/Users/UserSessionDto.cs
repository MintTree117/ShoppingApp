namespace Shop.Types.Users;

public readonly record struct UserSessionDto(
    string SessionId,
    DateTime LastActivityDate,
    string SessionInformation );