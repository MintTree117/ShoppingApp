using Shop.Infrastructure.Authentication;
using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;
using Shop.Infrastructure.Catalog.Products.Models;
using Shop.Infrastructure.Common.ReplyTypes;
using Shop.Infrastructure.Ordering.Types;

namespace Shop.Infrastructure.Ordering;

public sealed class CartManager( StorageService storage, HttpService http, AuthenticationStateManager auth ) // Singleton
{
    const string SummaryStorageKey = "CartSummary";
    
    readonly StorageService _storage = storage;
    readonly HttpService _http = http;
    readonly AuthenticationStateManager _auth = auth;
    readonly object _lock = new();
    
    bool _isBusy = false;
    CartItems? _summaryInMemory = null;
    
    public async Task<Reply<CartItems>> Get()
    {
        if (await AlreadyUpdating() && _summaryInMemory is not null)
            return Reply<CartItems>.Success( _summaryInMemory );
        
        var storageReply = await _storage.GetLocalStorage<CartItems>( SummaryStorageKey );
        if (!storageReply && !await IsAuthenticated())
            return storageReply;

        var postReply = await _http.PostAsyncAuthenticated<List<CartItem>>( Consts.ApiPostGetCart, storageReply.Data );
        if (!postReply)
            return Reply<CartItems>.NotFound();

        var summary = CartItems.With( postReply.Data );
        await _storage.SetLocalStorage( SummaryStorageKey, summary );
        return Reply<CartItems>.Success( summary );
    }
    public async Task<Reply<bool>> Add( ProductDetails p )
    {
        var cartReply = await Get();
        if (!cartReply)
            return IReply.Fail( cartReply );

        SetBusy( true );
        CartItems items = cartReply.Data;
        if (items.Contains( p.Id ))
            return IReply.Conflict( "Item already in cart." );
        
        var item = CartItem.FromProduct( p );
        items.Add( item );
        
        var storageTask = _storage.SetLocalStorage( SummaryStorageKey, items );
        var httpTask = await IsAuthenticated()
            ? _http.PutAsyncAuthenticated<bool>( Consts.ApiAddToCart, item.ProductId )
            : null;
        if (httpTask is null) await storageTask;
        else await Task.WhenAll( storageTask, httpTask );
        
        _summaryInMemory = items;
        SetBusy( false );

        return !storageTask.Result && httpTask is not null && !httpTask.Result 
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    public async Task<Reply<bool>> Update( CartProduct product )
    {
        var cartReply = await Get();
        if (!cartReply)
            return IReply.Fail( cartReply );
        
        SetBusy( true );
        CartItems items = cartReply.Data;
        if (!items.Contains( product.ProductId ))
            return IReply.Conflict( "Cart does not contain this item." );
        items.Set( CartItem.FromCartProduct( product ) );
        
        var storageTask = _storage.SetLocalStorage( SummaryStorageKey, items );
        var httpTask = await IsAuthenticated() ?
            _http.PutAsyncAuthenticated<bool>( Consts.ApiUpdateCart, CartItem.FromCartProduct( product ) ) 
            : null;
        if (httpTask is null) await storageTask;
        else await Task.WhenAll( storageTask, httpTask );
        
        _summaryInMemory = items;
        SetBusy( false );

        return !storageTask.Result && httpTask is not null && !httpTask.Result 
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    public async Task<Reply<bool>> UpdateBulk( List<CartProduct> products )
    {
        var cartReply = await Get();
        if (!cartReply)
            return IReply.Fail( cartReply );

        SetBusy( true );

        List<CartItem> items = cartReply.Data.Items;
        foreach ( CartProduct p in products )
        {
            var item = items.FirstOrDefault( i => i.ProductId == p.ProductId );
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
        _summaryInMemory = CartItems.With( items );
        SetBusy( false );

        return !storageTask.Result && httpTask is not null && !httpTask.Result
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    public async Task<Reply<bool>> Delete( CartProduct product )
    {
        var cartReply = await Get();
        if (!cartReply)
            return IReply.Fail( cartReply );
        
        SetBusy( true );
        CartItems items = cartReply.Data;
        if (!items.Contains( product.ProductId ))
            return IReply.Conflict( "Cart does not contain this item." );
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
            await Task.Delay( 1000 );
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
}