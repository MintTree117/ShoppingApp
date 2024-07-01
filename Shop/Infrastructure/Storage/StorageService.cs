using System.Text.Json;
using Microsoft.JSInterop;
using Shop.Types.Common.ReplyTypes;
using Shop.Utilities;

namespace Shop.Infrastructure.Storage;

public sealed class StorageService( IJSRuntime jsRuntime )
{
    readonly IJSRuntime _jsRuntime = jsRuntime;

    enum ActionType
    {
        Get, Set, Remove, Clear
    }
    const string JsLocalHelper = "localStorageHelper";
    const string JsSessionHelper = "sessionStorageHelper";
    static string LocalMethod( ActionType actionType ) =>
        $"{JsLocalHelper}.{actionType.ToString().ToLower()}";
    static string SessionMethod( ActionType actionType ) =>
        $"{JsSessionHelper}.{actionType.ToString().ToLower()}";
    static void StorageException( Exception e, ActionType actionType )
    {
        Logger.LogError( e, $"An exception occurred during a local storage operation: {actionType.ToString()}" );
    }
    static void LocalException( Exception e, ActionType actionType )
    {
        Logger.LogError( e, $"An exception occurred during a session storage operation: {actionType.ToString()}" );
    }
    
    // SESSION STORAGE
    public async Task<Reply<string>> GetLocalStorageString( string key )
    {
        try
        {
            string value = await _jsRuntime.InvokeAsync<string>( LocalMethod( ActionType.Get ), key );
            return string.IsNullOrWhiteSpace( value )
                ? Reply<string>.NotFound()
                : Reply<string>.Success( value );
        }
        catch ( Exception e )
        {
            LocalException( e, ActionType.Get );
            return Reply<string>.StorageError();
        }

    }
    public async Task<Reply<T>> GetLocalStorage<T>( string key )
    {
        try
        {
            string json = await _jsRuntime.InvokeAsync<string>( LocalMethod( ActionType.Get ), key );
            if (string.IsNullOrWhiteSpace( json ))
                return Reply<T>.NotFound();
            
            T? data = JsonSerializer.Deserialize<T>( json );
            return data is null
                ? Reply<T>.Invalid()
                : Reply<T>.Success( data );
        }
        catch ( Exception e )
        {
            LocalException( e, ActionType.Get );
            return Reply<T>.StorageError();
        }
    }
    public async Task<Reply<bool>> SetLocalStorageString( string key, string value )
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync( LocalMethod( ActionType.Set ), key, value );
            return IReply.Success();
        }
        catch ( Exception e )
        {
            LocalException( e, ActionType.Set );
            return IReply.StorageError();
        }
    }
    public async Task<Reply<bool>> SetLocalStorage<T>( string key, T value )
    {
        try
        {
            Console.WriteLine( $"mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm {value}" );
            string json = JsonSerializer.Serialize( value );
            Console.WriteLine($"mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm {json}");
            await _jsRuntime.InvokeVoidAsync( LocalMethod( ActionType.Set ), key, json );
            return IReply.Success();
        }
        catch ( Exception e )
        {
            LocalException( e, ActionType.Set );
            return IReply.StorageError();
        }
    }
    public async Task<Reply<bool>> RemoveLocalStorage( string key )
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync( LocalMethod( ActionType.Remove ), key );
            return IReply.Success();
        }
        catch ( Exception e )
        {
            LocalException( e, ActionType.Remove );
            return IReply.StorageError();
        }
    }
    public async Task<Reply<bool>> ClearLocalStorage()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync( LocalMethod( ActionType.Clear ) );
            return IReply.Success();
        }
        catch ( Exception e )
        {
            LocalException( e, ActionType.Remove );
            return IReply.StorageError();
        }
    }
    
    // SESSION STORAGE
    public async Task<Reply<string>> GetSessionStorageString( string key )
    {
        try
        {
            string value = await _jsRuntime.InvokeAsync<string>( SessionMethod( ActionType.Get ), key );
            return string.IsNullOrWhiteSpace( value )
                ? Reply<string>.NotFound()
                : Reply<string>.Success( value );
        }
        catch ( Exception e )
        {
            StorageException( e, ActionType.Get );
            return Reply<string>.StorageError();
        }

    }
    public async Task<Reply<T>> GetSessionStorage<T>( string key )
    {
        try
        {
            string json = await _jsRuntime.InvokeAsync<string>( SessionMethod( ActionType.Get ), key );
            if (string.IsNullOrWhiteSpace( json ))
                return Reply<T>.NotFound();
            
            T? data = JsonSerializer.Deserialize<T>( json );
            return data is null
                ? Reply<T>.Invalid()
                : Reply<T>.Success( data );
        }
        catch ( Exception e )
        {
            StorageException( e, ActionType.Get );
            return Reply<T>.StorageError();
        }
    }
    public async Task<Reply<bool>> SetSessionStorageString( string key, string value )
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync( SessionMethod( ActionType.Set ), key, value );
            return IReply.Success();
        }
        catch ( Exception e )
        {
            StorageException( e, ActionType.Set );
            return IReply.StorageError();
        }
    }
    public async Task<Reply<bool>> SetSessionStorage<T>( string key, T value )
    {
        try
        {
            string json = JsonSerializer.Serialize( value );
            await _jsRuntime.InvokeVoidAsync( SessionMethod( ActionType.Set ), key, json );
            return IReply.Success();
        }
        catch ( Exception e )
        {
            StorageException( e, ActionType.Set );
            return IReply.StorageError();
        }
    }
    public async Task<Reply<bool>> RemoveSessionStorage( string key )
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync( SessionMethod( ActionType.Remove ), key );
            return IReply.Success();
        }
        catch ( Exception e )
        {
            StorageException( e, ActionType.Remove );
            return IReply.StorageError();
        }
    }
    public async Task<Reply<bool>> ClearSessionStorage()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync( SessionMethod( ActionType.Clear ) );
            return IReply.Success();
        }
        catch ( Exception e )
        {
            StorageException( e, ActionType.Clear );
            return IReply.StorageError();
        }
    }
}