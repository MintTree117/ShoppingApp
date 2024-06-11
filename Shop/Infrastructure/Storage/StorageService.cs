using Blazored.LocalStorage;
using Shop.Infrastructure.Common.ReplyTypes;
using Shop.Utilities;

namespace Shop.Infrastructure.Storage;

public sealed class StorageService( IServiceProvider provider ) // singleton wrapper for structured replies
{
    const string ExceptionMessage = "STORAGE SERVICE EXCEPTION";
    readonly IServiceProvider _provider = provider;

    public async Task<Reply<string>> GetString( string key )
    {
        try {
            string? o = await GetStorage().GetItemAsync<string>( key );
            return string.IsNullOrWhiteSpace( o )
                ? Reply<string>.False( $"{key} not found in local storage." )
                : Reply<string>.True( o );
        }
        catch ( Exception e ) {
            Logger.LogError( e, ExceptionMessage );
            return Reply<string>.False( $"An exception occurred while trying to fetch key {key} AS STRING from storage." );
        }
    }
    public async Task<Reply<T>> Get<T>( string key )
    {
        try
        {
            var o = await GetStorage().GetItemAsync<T>( key );
            return o is null 
                ? Reply<T>.False( $"{key} not found in local storage." ) 
                : Reply<T>.True( o );
        }
        catch ( Exception e ) {
            Logger.LogError( e, ExceptionMessage );
            return Reply<T>.False( e, $"An exception occurred while trying to fetch key {key} from storage." );
        }
    }
    public async Task<Reply<bool>> Set<T>( string key, T value )
    {
        try {
            await GetStorage().SetItemAsync( key, value );
            return IReply.True();
        }
        catch ( Exception e ) {
            Logger.LogError( e, ExceptionMessage );
            return Reply<bool>.False( e, $"An exception occurred while trying to set key {key} and value {value?.ToString()} from storage." );
        }
    }
    public async Task<Reply<bool>> Remove( string key )
    {
        try {
            await GetStorage().RemoveItemAsync( key );
            return IReply.True();
        }
        catch ( Exception e ) {
            Logger.LogError( e, ExceptionMessage );
            return Reply<bool>.False( e, $"An exception occurred while trying to remove key {key} from storage." );
        }
    }
    
    ILocalStorageService GetStorage()
    {
        using AsyncServiceScope scope = _provider.CreateAsyncScope();
        return scope.ServiceProvider.GetService<ILocalStorageService>() ?? throw new Exception( "Failed to get ILocalStorageService from provider." );
    }
}