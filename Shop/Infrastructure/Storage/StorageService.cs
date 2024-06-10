using Blazored.LocalStorage;
using Shop.Infrastructure.Common.Optionals;
using Shop.Utilities;

namespace Shop.Infrastructure.Storage;

public sealed class StorageService( IServiceProvider provider ) // Singleton wrapper for structured replies
{
    readonly IServiceProvider _provider = provider;

    public async Task<Reply<string>> GetString( string key )
    {
        try {
            string? o = await GetStorage().GetItemAsync<string>( key );
            return o is null
                ? Reply<string>.None( $"{key} not found in local storage." )
                : Reply<string>.With( o ?? string.Empty );
        }
        catch ( Exception e ) {
            Console.WriteLine( e );
            return Reply<string>.None();
        }
    }
    public async Task<Reply<T>> Get<T>( string key )
    {
        try
        {
            T? o = await GetStorage().GetItemAsync<T>( key );

            return o is null 
                ? Reply<T>.None( $"{key} not found in local storage." ) 
                : Reply<T>.With( o );
        }
        catch ( Exception e ) {
            Logger.LogError( "STORAGE ERROR", e );
            return Reply<T>.Exception( e, $"An exception occurred while trying to fetch key {key} from storage." );
        }
    }
    public async Task<Reply<bool>> Set<T>( string key, T value )
    {
        try {
            await GetStorage().SetItemAsync( key, value );
            return IReply.Okay();
        }
        catch ( Exception e ) {
            Logger.LogError( "STORAGE ERROR", e );
            return Reply<bool>.Exception( e, $"An exception occurred while trying to set key {key} and value {value?.ToString()} from storage." );
        }
    }
    public async Task<Reply<bool>> Remove( string key )
    {
        try {
            await GetStorage().RemoveItemAsync( key );
            return IReply.Okay();
        }
        catch ( Exception e ) {
            Logger.LogError( "STORAGE ERROR", e );
            return Reply<bool>.Exception( e, $"An exception occurred while trying to remove key {key} from storage." );
        }
    }
    
    ILocalStorageService GetStorage()
    {
        using AsyncServiceScope scope = _provider.CreateAsyncScope();
        return scope.ServiceProvider.GetService<ILocalStorageService>() ?? throw new Exception( "Failed to get ILocalStorageService from provider." );
    }
}