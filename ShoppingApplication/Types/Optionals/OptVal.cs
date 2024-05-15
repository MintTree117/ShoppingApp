namespace ShoppingApplication.Types.Optionals;

public readonly record struct OptVal<T> : IOptional where T : struct
{
    readonly string? _message = string.Empty;
    readonly T? _val = null;

    OptVal( T? val )
    {
        _val = val;

        if (val is null)
            Problem = Problem.NotFound;
    }
    OptVal( Problem problem, string? message = null )
    {
        Problem = problem;
        _message = message;
    }
    OptVal( Exception e, Problem problem, string? message = null )
    {
        Problem = problem;
        _message = $"{message} : Exception : {e} : {e.Message}";
    }

    public T Value => _val ?? default;

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
    public bool Succeeds( out OptVal<T> self )
    {
        self = this;
        return Problem is Problem.None;
    }
    public bool Fails( out OptVal<T> self )
    {
        self = this;
        return Problem is not Problem.None;
    }
    public T Reduce( T orElse ) => _val ?? orElse;
    public T Reduce( Func<T> orElse ) => _val ?? orElse();
    public OptVal<T> Where( Func<T, bool> predicate ) =>
        _val.HasValue && predicate( _val.Value ) ? this : None();
    public OptVal<T> WhereNot( Func<T, bool> predicate ) =>
        _val.HasValue && !predicate( _val.Value ) ? this : None();
    public static OptVal<T> Success( T val ) => new( val );
    public static OptVal<T> Maybe( T? val ) => new( val );
    public static OptVal<T> None() => new( Problem.NotFound );
    public static OptVal<T> Failure( Problem er ) => new( er );
    public static OptVal<T> Failure( Problem er, string msg ) => new( er, msg );
    public static OptVal<T> Failure( IOptional opt ) => new( opt.Problem, opt.Message );
    public static OptVal<T> Exception( Exception ex, Problem er ) => new( ex, er );
    public static OptVal<T> Exception( Exception ex, Problem er, string msg ) => new( ex, er, msg );
}