namespace Shop.Infrastructure.Storage;

public readonly record struct MemoryCacheEntry<T>(
    T Data,
    DateTime Timestamp )
{
    public static MemoryCacheEntry<T> New( T data ) =>
        new( data, DateTime.Now );
    public bool Expired( TimeSpan timeSpan ) =>
        DateTime.Now - Timestamp > timeSpan;
}