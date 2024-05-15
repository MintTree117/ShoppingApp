namespace ShopApplication.Types.Optionals;

public readonly record struct OptObjs<T> : IOptional where T : class, new()
{
    readonly IEnumerable<T>? _enumerable = null;
    readonly string? _message = string.Empty;

    OptObjs( IEnumerable<T>? enumerable )
    {
        _enumerable = enumerable;

        if (_enumerable is null)
            Problem = Problem.NotFound;
    }
    OptObjs( Problem problem, string? message = null )
    {
        Problem = problem;
        _message = message;
    }
    OptObjs( Exception e, Problem problem, string? message = null )
    {
        Problem = problem;
        _message = $"{message} : Exception : {e} : {e.Message}";
    }

    public IEnumerable<T> Enumerable => _enumerable ?? Array.Empty<T>();

    public Problem Problem { get; init; } = Problem.None;
    public string Message => _message ?? string.Empty;

    public bool IsSuccess() => Problem is Problem.None;
    public string PrintDetails() => $"{Problem} : Message = {_message}";

    public bool Succeeds( out IOptional self )
    {
        self = this;
        return Problem is Problem.None;
    }
    public bool Fails( out IOptional self )
    {
        self = this;
        return Problem is not Problem.None;
    }
    public bool Succeeds( out OptObjs<T> self )
    {
        self = this;
        return Problem is Problem.None;
    }
    public bool Fails( out OptObjs<T> self )
    {
        self = this;
        return Problem is not Problem.None;
    }
    public static OptObjs<T> With( IEnumerable<T> objs ) => new( objs );
    public static OptObjs<T> Maybe( IEnumerable<T>? objs ) => new( objs );
    public static OptObjs<T> None() => new( Problem.NotFound );
    public static OptObjs<T> Error( Problem er ) => new( er );
    public static OptObjs<T> Error( Problem er, string msg ) => new( er, msg );
    public static OptObjs<T> Error( IOptional opt ) => new( opt.Problem, opt.Message );
    public static OptObjs<T> Exception( Exception ex, Problem er ) => new( ex, er );
    public static OptObjs<T> Exception( Exception ex, Problem er, string msg ) => new( ex, er, msg );
}