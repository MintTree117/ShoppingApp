namespace ShopApplication.Common.Optionals;

public interface IOptional
{
    public Problem Problem { get; init; }
    public string? Message { get; }

    public bool IsSuccess();
    public string PrintDetails();

    public static Val<bool> Success() => Val<bool>.Has( true );
    public static Val<bool> Failure( IOptional other ) => Val<bool>.Failure( other );
    public static Val<bool> Failure( Problem error, string message ) => Val<bool>.Failure( error, message );
}