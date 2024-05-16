namespace ShopApplication.Common.Optionals;

public readonly record struct Val<T> : IOptional where T : struct
{
    readonly string? _message = string.Empty;
    readonly T? _val = null;

    Val( T? val )
    {
        _val = val;

        if (val is null)
            Problem = Problem.NotFound;
    }
    Val( Problem problem, string? message = null )
    {
        Problem = problem;
        _message = message;
    }
    Val( Exception e, Problem problem, string? message = null )
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
    public bool Succeeds( out Val<T> self )
    {
        self = this;
        return Problem is Problem.None;
    }
    public bool Fails( out Val<T> self )
    {
        self = this;
        return Problem is not Problem.None;
    }
    public T Reduce( T orElse ) => _val ?? orElse;
    public T Reduce( Func<T> orElse ) => _val ?? orElse();
    public Val<T> Where( Func<T, bool> predicate ) =>
        _val.HasValue && predicate( _val.Value ) ? this : None();
    public Val<T> WhereNot( Func<T, bool> predicate ) =>
        _val.HasValue && !predicate( _val.Value ) ? this : None();
    public static Val<T> Has( T val ) => new( val );
    public static Val<T> Maybe( T? val ) => new( val );
    public static Val<T> None() => new( Problem.NotFound );
    public static Val<T> Failure( Problem er ) => new( er );
    public static Val<T> Failure( Problem er, string msg ) => new( er, msg );
    public static Val<T> Failure( IOptional opt ) => new( opt.Problem, opt.Message );
    public static Val<T> Exception( Exception ex, Problem er ) => new( ex, er );
    public static Val<T> Exception( Exception ex, Problem er, string msg ) => new( ex, er, msg );
}