using ShopApplication.Types.Optionals;

namespace ShopApplication.Infrastructure.Storage;

public interface IStorageService
{
    public Task<OptVal<T>> Get<T>( string key ) where T : struct;
    public Task<OptVal<bool>> Set<T>( string key, T value ) where T : struct;
    public Task<OptVal<bool>> Remove( string key );
}