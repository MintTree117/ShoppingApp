namespace ShopApplication.Types.Optionals;

public readonly record struct Obj<T> : IOptional where T : class, new()
{
    readonly string? _message = null;
    readonly T? _obj = null;

    Obj( T? obj )
    {
        _obj = obj;

        if (obj is null)
            Problem = Problem.NotFound;
    }
    Obj( Problem problem, string? message = null )
    {
        Problem = problem;
        _message = message;
    }
    Obj( Exception e, Problem problem, string? message = null )
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
    public bool Succeeds( out Obj<T> self )
    {
        self = this;
        return Problem is Problem.None;
    }
    public bool Fails( out Obj<T> self )
    {
        self = this;
        return Problem is not Problem.None;
    }
    public T Reduce( T orElse ) => _obj ?? orElse;
    public T Reduce( Func<T> orElse ) => _obj ?? orElse();
    public Obj<T> Where( Func<T, bool> predicate ) =>
        _obj is not null && predicate( _obj ) ? this : None();
    public Obj<T> WhereNot( Func<T, bool> predicate ) =>
        _obj is not null && !predicate( _obj ) ? this : None();
    public static Obj<T> Success( T obj ) => new( obj );
    public static Obj<T> Maybe( T? obj ) => new( obj );
    public static Obj<T> None() => new( Problem.NotFound );
    public static Obj<T> Failure( Problem er ) => new( er );
    public static Obj<T> Failure( Problem er, string msg ) => new( er, msg );
    public static Obj<T> Failure( IOptional opt ) => new( opt.Problem, opt.Message );
    public static Obj<T> Exception( Exception ex, Problem er ) => new( ex, er );
    public static Obj<T> Exception( Exception ex, Problem er, string msg ) => new( ex, er, msg );
}