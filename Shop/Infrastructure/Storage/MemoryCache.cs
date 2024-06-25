using Shop.Infrastructure.Common.ReplyTypes;

namespace Shop.Infrastructure.Storage;

public abstract class MemoryCache<T> // singleton that handles both storage and in-memory caching
    ( string storageKey, StorageService storage, TimeSpan cacheLife )
{
    readonly string _storageKey = storageKey;
    readonly StorageService _storage = storage;
    readonly TimeSpan _cacheLife = cacheLife;
    
    MemoryCacheEntry<T>? _inMemory;

    protected async Task<Reply<bool>> SetCache( T newData )
    {
        _inMemory = MemoryCacheEntry<T>.New( newData );
        return await _storage.SetLocalStorage( _storageKey, _inMemory );
    }
    protected async Task<Reply<T>> GetCache()
    {
        if (_inMemory is not null)
            if (_inMemory.Value.Expired( _cacheLife )) _inMemory = null;
            else return Reply<T>.Success( _inMemory.Value.Data );
        
        var storageReply = await _storage.GetLocalStorage<MemoryCacheEntry<T>>( _storageKey );
        if (!storageReply)
            return Reply<T>.NotFound( "Get Cache Entry Failed" );

        if (storageReply.Data.Expired( _cacheLife ))
            return Reply<T>.Invalid( "Cache Entry Expired." );

        _inMemory = storageReply.Data;
        return Reply<T>.Success( _inMemory.Value.Data );
    }
}