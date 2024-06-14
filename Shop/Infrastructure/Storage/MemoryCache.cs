using Shop.Infrastructure.Common.ReplyTypes;

namespace Shop.Infrastructure.Storage;

public abstract class MemoryCache<T> // singleton that handles both storage and in-memory caching
    ( string storageKey, StorageService storage, TimeSpan cacheLife )
{
    readonly string _storageKey = storageKey;
    readonly StorageService _storage = storage;
    readonly TimeSpan _cacheLife = cacheLife;
    
    MemoryCacheEntry<T>? _cacheEntry;

    protected async Task<Reply<bool>> SetCache( T newData )
    {
        _cacheEntry = MemoryCacheEntry<T>.New( newData );
        return await _storage.Set( _storageKey, _cacheEntry );
    }
    protected async Task<Reply<T>> GetCache()
    {
        if (_cacheEntry is not null)
            if (_cacheEntry.Value.Expired( _cacheLife )) _cacheEntry = null;
            else return Reply<T>.Success( _cacheEntry.Value.Data );
        
        Reply<MemoryCacheEntry<T>> storageReply = await _storage.Get<MemoryCacheEntry<T>>( _storageKey );
        if (!storageReply)
            return Reply<T>.Fail( storageReply, "Get Cache Entry Failed" );

        if (storageReply.Data.Expired( _cacheLife ))
            return Reply<T>.Fail( "Cache Entry Expired." );

        _cacheEntry = storageReply.Data;
        return Reply<T>.Success( _cacheEntry.Value.Data );
    }
}