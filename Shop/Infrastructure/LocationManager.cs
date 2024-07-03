using Shop.Infrastructure.Storage;
using Shop.Types.Common.ReplyTypes;
using Shop.Types.Users;

namespace Shop.Infrastructure;

public sealed class LocationManager( StorageService storage )
    : MemoryCache<AddressModel>( "Location", storage, TimeSpan.FromDays( 1 ) )
{
    public event Func<AddressModel?, Task>? OnLocationChanged; 
    
    public async Task<AddressModel?> GetCurrentAddress()
    {
        Reply<AddressModel> reply = await GetCache();
        return reply
            ? reply.Data
            : null;
    }
    public async Task<bool> SetCurrentLocation( AddressModel? address )
    {
        var reply = await SetCache( address );
        OnLocationChanged?.Invoke( address );
        return reply;
    }
    public async Task<bool> SetCurrentLocation( int? posX, int? posY )
    {
        AddressModel? address = posX.HasValue && posY.HasValue
            ? new AddressModel( Guid.Empty, "Custom Address", posX.Value, posY.Value )
            : null;
        
        var reply = await SetCache( address );
        OnLocationChanged?.Invoke( address );
        return reply;
    }
}