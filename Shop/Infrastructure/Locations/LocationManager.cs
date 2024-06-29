using Shop.Infrastructure.Storage;

namespace Shop.Infrastructure.Locations;

public sealed class LocationManager( StorageService storage )
    : MemoryCache<Location?>( "Location", storage, TimeSpan.FromDays( 1 ) )
{
    public event Func<Location?, Task>? OnLocationChanged; 
    
    public async Task<Location?> GetLocation()
    {
        var reply = await GetCache();
        return reply
            ? reply.Data
            : null;
    }
    public async Task<bool> SetLocation( int? posX, int? posY )
    {
        Location? l = posX is not null && posY is not null
            ? Location.With( posX.Value, posY.Value )
            : null;
        
        var reply = await SetCache( l );
        OnLocationChanged?.Invoke( l );
        return reply;
    }
}