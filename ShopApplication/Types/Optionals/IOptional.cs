namespace ShopApplication.Types.Optionals;

public interface IOptional
{
    public Problem Problem { get; init; }
    public string? Message { get; }

    public bool IsSuccess();
    public string PrintDetails();

    public static OptVal<bool> Success() => OptVal<bool>.Success( true );
    public static OptVal<bool> Failure( IOptional other ) => OptVal<bool>.Failure( other );
    public static OptVal<bool> Failure( Problem error, string message ) => OptVal<bool>.Failure( error, message );
}