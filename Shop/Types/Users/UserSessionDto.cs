namespace Shop.Types.Users;

public readonly record struct UserSessionDto(
    Guid SessionId,
    DateTime LastActivityDate,
    string SessionInformation );