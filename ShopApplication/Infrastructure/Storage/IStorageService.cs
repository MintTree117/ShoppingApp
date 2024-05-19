using RetailDomain.Optionals;

namespace ShopApplication.Infrastructure.Storage;

public interface IStorageService
{
    public Task<Opt<T>> Get<T>( string key );
    public Task<Opt<bool>> Set<T>( string key, T value );
    public Task<Opt<bool>> Remove( string key );
}