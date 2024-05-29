using Blazored.LocalStorage;
using Shop.Infrastructure.Common.Optionals;

namespace Shop.Infrastructure.Storage;

public sealed class StorageService( ILocalStorageService storageService )
{
    readonly ILocalStorageService storage = storageService;

    public async Task<Opt<string>> GetString( string key )
    {
        try {
            string? o = await storage.GetItemAsync<string>( key );
            return o is null
                ? Opt<string>.None( $"{key} not found in local storage." )
                : Opt<string>.With( o );
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return Opt<string>.Exception( e, $"An exception occurred while trying to fetch key {key} from storage." );
        }
    }
    public async Task<Opt<T>> Get<T>( string key )
    {
        try {
            var o = await storage.GetItemAsync<T>( key );
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
            await storage.SetItemAsync( key, value );
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
            await storage.RemoveItemAsync( key );
            return IOpt.Okay();
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return Opt<bool>.Exception( e, $"An exception occurred while trying to remove key {key} from storage." );
        }
    }
}