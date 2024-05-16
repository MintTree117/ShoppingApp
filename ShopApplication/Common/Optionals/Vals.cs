namespace ShopApplication.Common.Optionals;

public readonly record struct Vals<T> : IOptional where T : struct
{
    readonly IEnumerable<T>? _enumerable = null;
    readonly string? _message = string.Empty;

    Vals( IEnumerable<T>? enumerable )
    {
        _enumerable = enumerable;

        if (_enumerable is null)
            Problem = Problem.NotFound;
    }
    Vals( Problem problem, string? message = null )
    {
        Problem = problem;
        _message = message;
    }
    Vals( Exception e, Problem problem, string? message = null )
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
    public bool Succeeds( out Vals<T> self )
    {
        self = this;
        return Problem is Problem.None;
    }
    public bool Fails( out Vals<T> self )
    {
        self = this;
        return Problem is not Problem.None;
    }
    public static Vals<T> With( IEnumerable<T> objs ) => new( objs );
    public static Vals<T> Maybe( IEnumerable<T>? objs ) => new( objs );
    public static Vals<T> None() => new( Problem.NotFound );
    public static Vals<T> Error( Problem er ) => new( er );
    public static Vals<T> Error( Problem er, string msg ) => new( er, msg );
    public static Vals<T> Error( IOptional opt ) => new( opt.Problem, opt.Message );
    public static Vals<T> Exception( Exception ex, Problem er ) => new( ex, er );
    public static Vals<T> Exception( Exception ex, Problem er, string msg ) => new( ex, er, msg );
}