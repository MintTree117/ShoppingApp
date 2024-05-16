using ShopApplication.Types.Optionals;

namespace ShopApplication.Infrastructure.Storage;

public interface IStorageService
{
    public Task<Val<T>> Get<T>( string key ) where T : struct;
    public Task<Val<bool>> Set<T>( string key, T value ) where T : struct;
    public Task<Val<bool>> Remove( string key );
}