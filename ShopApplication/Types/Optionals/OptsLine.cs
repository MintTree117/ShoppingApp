namespace ShopApplication.Types.Optionals;

public readonly record struct OptsLine<T>(
    List<OptObj<T>> Options ) where T : class, new()
{
    public bool Succeeds( out OptsLine<T> self )
    {
        self = this;
        return !AnyFailed( out self );
    }
    public bool AnySucceeded => Options.Any( o => o.IsSuccess() );
    public bool AnyFailed( out OptsLine<T> self )
    {
        self = this;
        return Options.Any( o => !o.IsSuccess() );
    }
    public List<T> ToObjects() => Options.Select( o => o.Object ).ToList();
}