using System.Text.Json;
using Microsoft.JSInterop;
using ShoppingApplication.Types;
using ShoppingApplication.Types.Optionals;

namespace ShoppingApplication.Infrastructure.Storage;

internal sealed class StorageService( IJSRuntime jsRuntime )
{
    readonly IJSRuntime _jsRuntime = jsRuntime;
    
    public async Task<OptVal<T>> Get<T>( string key ) where T : struct
    {
        try {
            string jsonString = await _jsRuntime.InvokeAsync<string>( "localStorage.getItem", key );
            return string.IsNullOrEmpty( jsonString )
                ? OptVal<T>.Failure( Problem.IO )
                : OptVal<T>.Success( JsonSerializer.Deserialize<T>( jsonString ) );
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return OptVal<T>.Exception( e, Problem.IO, $"An exception occurred while trying to fetch key {key} from storage." );
        }
    }
    public async Task<OptVal<bool>> Set<T>( string key, T value ) where T : struct
    {
        try {
            string jsonString = JsonSerializer.Serialize( value );
            await _jsRuntime.InvokeVoidAsync( "localStorage.setItem", key, jsonString );
            return IOptional.Success();
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return OptVal<bool>.Exception( e, Problem.IO, $"An exception occurred while trying to set key {key} and value {value.ToString()} from storage." );
        }
    }
    public async Task<OptVal<bool>> Remove( string key )
    {
        try {
            await _jsRuntime.InvokeVoidAsync( "localStorage.removeItem", key );
            return IOptional.Success();
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return OptVal<bool>.Exception( e, Problem.IO, $"An exception occurred while trying to remove key {key} from storage." );
        }
    }
}