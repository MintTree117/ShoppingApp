using System.Text.Json;
using Microsoft.JSInterop;
using RetailDomain.Optionals;

namespace ShopApplication.Infrastructure.Storage;

internal sealed class StorageService( IJSRuntime jsRuntime ) : IStorageService
{
    readonly IJSRuntime _jsRuntime = jsRuntime;

    public async Task<Opt<T>> Get<T>( string key )
    {
        try {
            string jsonString = await _jsRuntime.InvokeAsync<string>( "localStorage.getItem", key );
            var o = JsonSerializer.Deserialize<T>( jsonString );
            return o is null
                ? Opt<T>.None( $"{key} not found in local storage." )
                : Opt<T>.With( o );
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return Opt<T>.Exception( e, $"An exception occurred while trying to fetch key {key} from storage." );
        }
    }
    public async Task<Opt<bool>> Set<T>( string key, T value )
    {
        try {
            string jsonString = JsonSerializer.Serialize( value );
            await _jsRuntime.InvokeVoidAsync( "localStorage.setItem", key, jsonString );
            return IOpt.Okay();
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return Opt<bool>.Exception( e, $"An exception occurred while trying to set key {key} and value {value?.ToString()} from storage." );
        }
    }
    public async Task<Opt<bool>> Remove( string key )
    {
        try {
            await _jsRuntime.InvokeVoidAsync( "localStorage.removeItem", key );
            return IOpt.Okay();
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return Opt<bool>.Exception( e, $"An exception occurred while trying to remove key {key} from storage." );
        }
    }
}