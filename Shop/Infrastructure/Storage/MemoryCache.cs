using Shop.Infrastructure.Common.Optionals;

namespace Shop.Infrastructure.Storage;

public abstract class MemoryCache<T> // singleton that handles both storage and in-memory caching
{
    readonly string StorageKey;
    readonly StorageService Storage;
    readonly TimeSpan CacheLife;
    
    MemoryCacheEntry<T>? _cacheEntry;

    public MemoryCache( string storageKey, StorageService storage, TimeSpan cacheLife )
    {
        StorageKey = storageKey;
        Storage = storage;
        CacheLife = cacheLife;
    }

    public async Task<Reply<bool>> Set( T newData )
    {
        _cacheEntry = MemoryCacheEntry<T>.New( newData );
        return await Storage.Set( StorageKey, _cacheEntry );

    }
    public async Task<Reply<T>> Get()
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