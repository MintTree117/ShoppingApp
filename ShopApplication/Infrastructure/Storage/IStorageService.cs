using ShopApplication.Common.Optionals;

namespace ShopApplication.Infrastructure.Storage;

public interface IStorageService
{
    public Task<Obj<T>> GetObj<T>( string key ) where T : class;
    public Task<Val<T>> GetVal<T>( string key ) where T : struct;
    public Task<Val<bool>> Set<T>( string key, T value );
    public Task<Val<bool>> Remove( string key );
}