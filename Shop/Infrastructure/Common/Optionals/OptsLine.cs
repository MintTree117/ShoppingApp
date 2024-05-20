namespace Shop.Infrastructure.Common.Optionals;

public readonly record struct OptsLine<T>(
    List<Opt<T>> Options ) where T : class, new()
{
    public bool Succeeds( out OptsLine<T> self )
    {
        self = this;
        return !AnyFailed( out self );
    }
    public bool AnySucceeded => Options.Any( o => o.IsOkay );
    public bool AnyFailed( out OptsLine<T> self )
    {
        self = this;
        return Options.Any( o => !o.IsOkay );
    }
    public List<T> ToObjects() => Options.Select( o => o.Data ).ToList();
}