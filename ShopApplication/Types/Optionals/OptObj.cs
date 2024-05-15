namespace ShopApplication.Types.Optionals;

public readonly record struct OptObj<T> : IOptional where T : class, new()
{
    readonly string? _message = null;
    readonly T? _obj = null;

    OptObj( T? obj )
    {
        _obj = obj;

        if (obj is null)
            Problem = Problem.NotFound;
    }
    OptObj( Problem problem, string? message = null )
    {
        Problem = problem;
        _message = message;
    }
    OptObj( Exception e, Problem problem, string? message = null )
    {
        Problem = problem;
        _message = $"{message} : Exception : {e} : {e.Message}";
    }

    public T Object => _obj ?? new T();

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
    public bool Succeeds( out OptObj<T> self )
    {
        self = this;
        return Problem is Problem.None;
    }
    public bool Fails( out OptObj<T> self )
    {
        self = this;
        return Problem is not Problem.None;
    }
    public T Reduce( T orElse ) => _obj ?? orElse;
    public T Reduce( Func<T> orElse ) => _obj ?? orElse();
    public OptObj<T> Where( Func<T, bool> predicate ) =>
        _obj is not null && predicate( _obj ) ? this : None();
    public OptObj<T> WhereNot( Func<T, bool> predicate ) =>
        _obj is not null && !predicate( _obj ) ? this : None();
    public static OptObj<T> Success( T obj ) => new( obj );
    public static OptObj<T> Maybe( T? obj ) => new( obj );
    public static OptObj<T> None() => new( Problem.NotFound );
    public static OptObj<T> Failure( Problem er ) => new( er );
    public static OptObj<T> Failure( Problem er, string msg ) => new( er, msg );
    public static OptObj<T> Failure( IOptional opt ) => new( opt.Problem, opt.Message );
    public static OptObj<T> Exception( Exception ex, Problem er ) => new( ex, er );
    public static OptObj<T> Exception( Exception ex, Problem er, string msg ) => new( ex, er, msg );
}