using Shop.Infrastructure.Storage;
using Shop.Types.Common.ReplyTypes;
using Shop.Types.Users;

namespace Shop.Infrastructure;

public sealed class LocationManager( StorageService storage )
    : MemoryCache<Address>( "Location", storage, TimeSpan.FromDays( 1 ) )
{
    public event Func<Address?, Task>? OnLocationChangedAsync;
    public event Action<Address?>? OnLocationChanged;
    public event Func<Task>? OnOpenAddressBox;

    public void OpenAddressBox()
    {
        OnOpenAddressBox?.Invoke();
    }
    public async Task<Address?> GetCurrentAddress()
    {
        Reply<Address> reply = await GetCache();
        return reply
            ? reply.Data
            : null;
    }
    public async Task<bool> SetCurrentLocation( Address? address )
    {
        var reply = await SetCache( address );
        OnLocationChanged?.Invoke(address);
        OnLocationChangedAsync?.Invoke( address );
        return reply;
    }
    public async Task<bool> SetCurrentLocation( int? posX, int? posY )
    {
        Address? address = posX.HasValue && posY.HasValue
            ? new Address( "Custom Address", posX.Value, posY.Value )
            : null;
        
        var reply = await SetCache( address );
        OnLocationChanged?.Invoke( address );
        OnLocationChangedAsync?.Invoke( address );
        return reply;
    }
}