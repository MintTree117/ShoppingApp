using Shop.Infrastructure.Common;

namespace Shop.Shared;

public readonly record struct PushAlertArgs(
    AlertType Type,
    string Message )
{
    internal static PushAlertArgs With( AlertType type, string message ) =>
        new( type, message );
}
public readonly record struct NavigationArgs(
    string Url,
    bool ForceReload = false )
{
    internal static NavigationArgs With( string url, bool isReal ) =>
        new( url, isReal );
}