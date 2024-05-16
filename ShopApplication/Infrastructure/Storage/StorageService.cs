using System.Text.Json;
using Microsoft.JSInterop;
using ShopApplication.Types;
using ShopApplication.Types.Optionals;

namespace ShopApplication.Infrastructure.Storage;

internal sealed class StorageService( IJSRuntime jsRuntime ) : IStorageService
{
    readonly IJSRuntime _jsRuntime = jsRuntime;
    
    public async Task<Val<T>> Get<T>( string key ) where T : struct
    {
        try {
            string jsonString = await _jsRuntime.InvokeAsync<string>( "localStorage.getItem", key );
            return string.IsNullOrEmpty( jsonString )
                ? Val<T>.Failure( Problem.IO )
                : Val<T>.Has( JsonSerializer.Deserialize<T>( jsonString ) );
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return Val<T>.Exception( e, Problem.IO, $"An exception occurred while trying to fetch key {key} from storage." );
        }
    }
    public async Task<Val<bool>> Set<T>( string key, T value ) where T : struct
    {
        try {
            string jsonString = JsonSerializer.Serialize( value );
            await _jsRuntime.InvokeVoidAsync( "localStorage.setItem", key, jsonString );
            return IOptional.Success();
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return Val<bool>.Exception( e, Problem.IO, $"An exception occurred while trying to set key {key} and value {value.ToString()} from storage." );
        }
    }
    public async Task<Val<bool>> Remove( string key )
    {
        try {
            await _jsRuntime.InvokeVoidAsync( "localStorage.removeItem", key );
            return IOptional.Success();
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return Val<bool>.Exception( e, Problem.IO, $"An exception occurred while trying to remove key {key} from storage." );
        }
    }
}