using Microsoft.AspNetCore.Components.Authorization;
using Shop.Infrastructure.Authentication;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Types.Cart;
using Shop.Types.Common.ReplyTypes;
using Shop.Utilities;

namespace Shop.Infrastructure;

public sealed class CartManager : IDisposable // Singleton
{
    const string SummaryStorageKey = "CartSummary";

    readonly NotificationService _notification;
    readonly StorageService _storage;
    readonly HttpService _http;
    readonly AuthenticationStateManager _auth;
    readonly object _lock = new();

    public event Action<CartItems?>? OnCartChanged;

    bool _isAuthenticated = false;
    bool _isBusy = false;
    CartItems? _summaryInMemory;
    
    public CartManager( NotificationService notification, StorageService storage, HttpService http, AuthenticationStateManager auth )
    {
        _notification = notification;
        _storage = storage;
        _http = http;
        _auth = auth;
        _auth.OnStateChanged += OnAuthenticationStateChange;
        
        GetInitialAuthState();

        async void GetInitialAuthState()
        {
            var state = await _auth.GetSessionState();
            _isAuthenticated =
                state.User.Identity is not null &&
                state.User.Identity.IsAuthenticated;
        }
    }
    public void Dispose()
    {
        _auth.OnStateChanged -= OnAuthenticationStateChange;
    }

    void OnAuthenticationStateChange( Task<AuthenticationState> task )
    {
        _isAuthenticated =
            task.Result.User.Identity is not null &&
            task.Result.User.Identity.IsAuthenticated;
    }

    public async Task<Reply<CartItems>> GetInMemory()
    {
        await Task.Delay( 200 );
        await AlreadyUpdating();
        return _summaryInMemory is not null
            ? Reply<CartItems>.Success( _summaryInMemory )
            : Reply<CartItems>.NotFound();
    }
    public async Task<Reply<CartItems>> GetFull()
    {
        await Task.Delay( 200 );
        if (await AlreadyUpdating() && _summaryInMemory is not null)
            return Reply<CartItems>.Success( _summaryInMemory );
        
        SetBusy( true );
        
        var storageReply = await _storage.GetLocalStorage<CartItems>( SummaryStorageKey );
        if (!_isAuthenticated)
        {
            _summaryInMemory = storageReply
                ? storageReply.Data
                : null;
            SetBusy( false );
            InvokeCartChange();
            return Reply<CartItems>.Success( _summaryInMemory ?? CartItems.Empty() );
        }
        
        var serverReply = await _http.PostAsyncAuthenticated<List<CartItemDto>>(
            _http.OrderingApi( Consts.ApiPostGetCart ), storageReply ? storageReply.Data.Items : [] );
        if (!serverReply)
        {
            SetBusy( false );
            InvokeCartChange();
            return Reply<CartItems>.Fail( serverReply );
        }

        _summaryInMemory = CartItems.With( serverReply.Data );
        await _storage.SetLocalStorage( SummaryStorageKey, _summaryInMemory );
        SetBusy( false );
        InvokeCartChange();
        return Reply<CartItems>.Success( _summaryInMemory );
    }
    public async Task<Reply<bool>> Add( Guid id, int quantity = 1 )
    {
        SetBusy( true );
        await Task.Delay( 200 );
        Reply<CartItems> items = await _storage.GetLocalStorage<CartItems>( SummaryStorageKey );
        _summaryInMemory = items
            ? items.Data
            : CartItems.Empty();
        _summaryInMemory.Add( new CartItemDto( id, quantity ) );

        return await Update( _summaryInMemory.Items );
    }
    public async Task<Reply<bool>> Update( List<CartItemDto> items )
    {
        SetBusy( true );
        await Task.Delay( 200 );
        _summaryInMemory = new CartItems( items );

        var storageTask = _storage.SetLocalStorage( SummaryStorageKey, _summaryInMemory );
        var httpTask = _isAuthenticated
            ? _http.PostAsyncAuthenticated<List<CartItemDto>>( _http.OrderingApi( Consts.ApiPostGetCart ), _summaryInMemory )
            : null;

        if (httpTask is null) await storageTask;
        else await Task.WhenAll( storageTask, httpTask );

        if (httpTask is not null && httpTask.Result)
        {
            _summaryInMemory = new CartItems( httpTask.Result.Data );
            await _storage.SetLocalStorage( SummaryStorageKey, _summaryInMemory );
        }

        SetBusy( false );
        InvokeCartChange();

        var failed = !storageTask.Result && httpTask is not null && !httpTask.Result;
        if (failed) _notification.PushError( "Failed to update cart." );
        else _notification.PushSuccess( "Updated cart." );
        return failed
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    public async Task<Reply<bool>> Clear()
    {
        SetBusy( true );
        await Task.Delay( 500 );
        _summaryInMemory = null;
        var storageTask = _storage.RemoveLocalStorage( SummaryStorageKey );
        var httpTask = _isAuthenticated
            ? _http.DeleteAsyncAuthenticated<bool>( _http.OrderingApi( Consts.ApiClearCart ) )
            : null;

        if (httpTask is null) await storageTask;
        else await Task.WhenAll( storageTask, httpTask );
        
        InvokeCartChange();
        SetBusy( false );

        var failed = !storageTask.Result && httpTask is not null && !httpTask.Result;
        if (failed) _notification.PushError( "Failed to clear cart." );
        else _notification.PushSuccess( "Cleared cart." );
        return failed
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    
    async Task<bool> AlreadyUpdating()
    {
        bool wasBusy = false;
        int iterations = 0;
        while ( _isBusy && iterations < 3 )
        {
            await Task.Delay( 200 );
            iterations++;
            wasBusy = true;
        }
        return wasBusy;
    }
    void SetBusy( bool value )
    {
        lock ( _lock )
            _isBusy = value;
    }
    void InvokeCartChange()
    {
        OnCartChanged?.Invoke( _summaryInMemory );
    }
}