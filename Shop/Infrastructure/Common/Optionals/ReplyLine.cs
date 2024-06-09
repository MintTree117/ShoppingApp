namespace Shop.Infrastructure.Common.Optionals;

public readonly record struct ReplyLine<T>(
    List<Reply<T>> Options ) where T : class, new()
{
    public bool Succeeds( out ReplyLine<T> self )
    {
        self = this;
        return !AnyFailed( out self );
    }
    public bool AnySucceeded => Options.Any( o => o.IsOkay );
    public bool AnyFailed( out ReplyLine<T> self )
    {
        self = this;
        return Options.Any( o => !o.IsOkay );
    }
    public List<T> ToObjects() => Options.Select( o => o.Data ).ToList();
}