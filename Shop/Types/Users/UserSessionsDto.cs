namespace Shop.Types.Users;

public readonly record struct UserSessionsDto(
    int TotalCount,
    List<UserSessionDto> Sessions );