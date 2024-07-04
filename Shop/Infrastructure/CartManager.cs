using Shop.Infrastructure.Authentication;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Types.Cart;
using Shop.Types.Common.ReplyTypes;
using Shop.Utilities;

namespace Shop.Infrastructure;

public sealed class CartManager( StorageService storage, HttpService http, AuthenticationStateManager auth ) // Singleton
{
    const string SummaryStorageKey = "CartSummary";
    
    readonly StorageService _storage = storage;
    readonly HttpService _http = http;
    readonly AuthenticationStateManager _auth = auth;
    readonly object _lock = new();

    public event Action<int>? OnCartCountChanged; 
    
    bool _isBusy;
    CartItems? _summaryInMemory;
    
    public async Task<Reply<CartItems>> Get()
    {
        SetBusy( true );

        if (await AlreadyUpdating() && _summaryInMemory is not null)
        {
            SetBusy( false );
            return Reply<CartItems>.Success( _summaryInMemory );
        }
        
        var storageReply = await _storage.GetLocalStorage<CartItems>( SummaryStorageKey );
        if (!await IsAuthenticated())
        {
            _summaryInMemory = storageReply.Data;
            SetBusy( false );
            return storageReply;
        }
        
        var serverReply = await _http.PostAsyncAuthenticated<List<CartItem>>( 
            Consts.ApiPostGetCart, storageReply ? storageReply.Data : null );
        if (!serverReply)
        {
            SetBusy( false );
            InvokeCountChange();
            return Reply<CartItems>.Fail( serverReply );
        }

        _summaryInMemory = CartItems.With( serverReply.Data );
        await _storage.SetLocalStorage( SummaryStorageKey, _summaryInMemory );
        SetBusy( false );
        InvokeCountChange();
        return Reply<CartItems>.Success( _summaryInMemory );
    }
    public async Task<Reply<bool>> Add( Guid productId )
    {
        SetBusy( true );
        
        var cartReply = await Get();
        CartItems items = cartReply
            ? cartReply.Data
            : CartItems.Empty();
        
        if (items.Contains( productId ))
        {
            SetBusy( false );
            return IReply.Conflict( "Item already in cart." );
        }
        
        var item = new CartItem( productId, 1 );
        items?.Add( item );
        
        var storageTask = _storage.SetLocalStorage( SummaryStorageKey, items );
        var httpTask = await IsAuthenticated()
            ? _http.PutAsyncAuthenticated<bool>( Consts.ApiAddToCart, item.ProductId )
            : null;
        if (httpTask is null) await storageTask;
        else await Task.WhenAll( storageTask, httpTask );
        
        _summaryInMemory = items;
        SetBusy( false );
        InvokeCountChange();

        return !storageTask.Result && (httpTask is not null && !httpTask.Result )
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    public async Task<Reply<bool>> Update( CartProduct product )
    {
        SetBusy( true );
        
        var cartReply = await Get();
        CartItems? items = cartReply
            ? cartReply.Data
            : null;

        if (items is not null && !items.Contains( product.ProductId ))
        {
            SetBusy( false );
            return IReply.Conflict( "Cart does not contain this item." );
        }

        
        items?.Set( CartItem.FromCartProduct( product ) );
        
        var storageTask = _storage.SetLocalStorage( SummaryStorageKey, items );
        var httpTask = await IsAuthenticated() ?
            _http.PutAsyncAuthenticated<bool>( Consts.ApiUpdateCart, CartItem.FromCartProduct( product ) ) 
            : null;
        
        if (httpTask is null) await storageTask;
        else await Task.WhenAll( storageTask, httpTask );
        
        _summaryInMemory = items;
        SetBusy( false );
        InvokeCountChange();

        return !storageTask.Result && httpTask is not null && !httpTask.Result 
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    public async Task<Reply<bool>> UpdateBulk( List<CartProduct> products )
    {
        SetBusy( true );

        var cartReply = await Get();
        CartItems? items = cartReply
            ? cartReply.Data
            : null;

        foreach ( CartProduct p in products )
        {
            var item = items?.Items.FirstOrDefault( i => i.ProductId == p.ProductId );
            if (item is null)
                continue;
            item.Quantity = p.Quantity;
        }

        var storageTask = _storage.SetLocalStorage( SummaryStorageKey, items );
        var httpTask = await IsAuthenticated()
            ? _http.PutAsyncAuthenticated<bool>( Consts.ApiUpdateCartBulk, items )
            : null;

        if (httpTask is null) await storageTask;
        else await Task.WhenAll( storageTask, httpTask );
        _summaryInMemory = CartItems.With( items?.Items ?? [] );
        SetBusy( false );
        InvokeCountChange();

        return !storageTask.Result && httpTask is not null && !httpTask.Result
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    public async Task<Reply<bool>> Delete( CartProduct product )
    {
        SetBusy( true );

        var cartReply = await Get();
        CartItems items = cartReply
            ? cartReply.Data
            : CartItems.Empty();

        if (!items.Contains( product.ProductId ))
        {
            SetBusy( false );
            return IReply.Conflict( "Cart does not contain this item." );
        }

        items.Delete( product.ProductId );

        var storageTask = _storage.SetLocalStorage( SummaryStorageKey, items );
        var httpTask = await IsAuthenticated()
            ? _http.DeleteAsyncAuthenticated<bool>( Consts.ApiDeleteFromCart,
                new Dictionary<string, object>() { { "ProductId", product.ProductId } } )
            : null;
        
        if (httpTask is null) await storageTask;
        else await Task.WhenAll( storageTask, httpTask );
        
        _summaryInMemory = items;
        SetBusy( false );
        InvokeCountChange();

        return !storageTask.Result && httpTask is not null && !httpTask.Result 
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    public async Task<Reply<bool>> Clear()
    {
        SetBusy( true );
        
        _summaryInMemory = null;
        var storageTask = _storage.RemoveLocalStorage( SummaryStorageKey );
        var httpTask = await IsAuthenticated()
            ? _http.DeleteAsyncAuthenticated<bool>( Consts.ApiClearCart )
            : null;
        
        InvokeCountChange();
        SetBusy( false );

        return !storageTask.Result && httpTask is not null && !httpTask.Result
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    
    async Task<bool> AlreadyUpdating()
    {
        bool wasBusy = false;
        int iterations = 0;
        while ( _isBusy && iterations < 3 )
        {
            await Task.Delay( 300 );
            iterations++;
            wasBusy = true;
        }
        return wasBusy;
    }
    async Task<bool> IsAuthenticated()
    {
        var authState = await _auth.GetSessionState();
        var authenticated = authState.User.Identity is { IsAuthenticated: true };
        return authenticated;
    }
    void SetBusy( bool value )
    {
        lock ( _lock )
            _isBusy = value;
    }

    void InvokeCountChange()
    {
        OnCartCountChanged?.Invoke( _summaryInMemory?.Count() ?? 0 );
    }
}