using Shop.Infrastructure.Common.Optionals;

namespace Shop.Infrastructure.Storage;

public abstract class MemoryCache<T> // singleton that handles both storage and in-memory caching
{
    readonly string StorageKey;
    readonly StorageService Storage;
    readonly TimeSpan CacheLife;
    
    MemoryCacheEntry<T>? _cacheEntry;

    protected MemoryCache( string storageKey, StorageService storage, TimeSpan cacheLife )
    {
        StorageKey = storageKey;
        Storage = storage;
        CacheLife = cacheLife;
    }
    protected async Task<Reply<bool>> SetCache( T newData )
    {
        _cacheEntry = MemoryCacheEntry<T>.New( newData );
        return await Storage.Set( StorageKey, _cacheEntry );

    }
    protected async Task<Reply<T>> GetCache()
    {
        if (_cacheEntry is not null)
            if (_cacheEntry.Value.Expired( CacheLife )) _cacheEntry = null;
            else return Reply<T>.With( _cacheEntry.Value.Data );
        
        Reply<MemoryCacheEntry<T>> storageReply = await Storage.Get<MemoryCacheEntry<T>>( StorageKey );
        if (!storageReply.IsOkay)
            return Reply<T>.None( storageReply );

        if (storageReply.Data.Expired( CacheLife ))
            return Reply<T>.None( "Expired" );

        _cacheEntry = storageReply.Data;
        return Reply<T>.With( _cacheEntry.Value.Data );
    }
}