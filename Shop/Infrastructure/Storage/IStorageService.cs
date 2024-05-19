using Shop.Infrastructure.Common.Optionals;

namespace Shop.Infrastructure.Storage;

public interface IStorageService
{
    public Task<Opt<T>> Get<T>( string key );
    public Task<Opt<bool>> Set<T>( string key, T value );
    public Task<Opt<bool>> Remove( string key );
}