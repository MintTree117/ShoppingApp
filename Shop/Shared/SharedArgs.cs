using ShopApplication.Common;

namespace Shop.Shared;

public readonly record struct PushAlertArgs(
    AlertType Type,
    string Message )
{
    internal static PushAlertArgs With( AlertType type, string message ) =>
        new( type, message );
}
public readonly record struct NavigateToArgs(
    string Url,
    bool ForceReload )
{
    internal static NavigateToArgs With( string url, bool isReal ) =>
        new( url, isReal );
}