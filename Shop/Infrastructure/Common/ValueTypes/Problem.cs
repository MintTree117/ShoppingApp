namespace Shop.Infrastructure.Common.ValueTypes;

public enum Problem
{
    None,
    BadRequest,
    Validation,
    LockedOut,
    Unauthorized,
    Internal,
    Network,
    NotFound,
    Conflict
}