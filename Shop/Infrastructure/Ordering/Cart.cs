using Shop.Infrastructure.Http;
using Shop.Infrastructure.Storage;
using Shop.Utilities;
using Shop.Infrastructure.Catalog.Products.Models;
using Shop.Infrastructure.Common.ReplyTypes;
using Shop.Infrastructure.Ordering.Types;

namespace Shop.Infrastructure.Ordering;

public sealed class Cart( StorageService storage, HttpService http ) // Singleton
{
    const string SummaryStorageKey = "CartSummary";
    
    readonly StorageService _storage = storage;
    readonly HttpService _http = http;
    readonly object _lock = new();
    
    bool _isBusy = false;
    CartSummary? _summaryInMemory = null;
    
    public async Task<Reply<CartSummary>> Get( bool authenticated )
    {
        if (await IsBusy() && _summaryInMemory is not null)
            return Reply<CartSummary>.Success( _summaryInMemory );

        var storageReply = await _storage.GetLocalStorage<CartSummary>( SummaryStorageKey );
        if (!storageReply && !authenticated)
            return storageReply;

        var postReply = await _http.PostAsyncAuthenticated<List<CartItemDto>>( Consts.ApiPostGetCart, storageReply.Data );
        if (!postReply)
            return Reply<CartSummary>.NotFound();

        var summary = CartSummary.With( postReply.Data );
        await _storage.SetLocalStorage( SummaryStorageKey, summary );
        return Reply<CartSummary>.Success( summary );
    }
    public async Task<Reply<bool>> Add( Product p, bool authenticated )
    {
        var cartReply = await Get( authenticated );
        if (!cartReply)
            return IReply.Fail( cartReply );

        SetBusy( true );
        CartSummary summary = cartReply.Data;
        if (summary.Contains( p.Id ))
            return IReply.Conflict( "Item already in cart." );
        
        var dto = CartItemDto.FromProduct( p );
        summary.Add( dto );
        
        var storageTask = _storage.SetLocalStorage( SummaryStorageKey, summary );
        var httpTask = _http.PutAsyncAuthenticated<bool>( Consts.ApiAddToCart, dto );
        await Task.WhenAll( storageTask, httpTask );
        
        _summaryInMemory = summary;
        SetBusy( false );

        return !storageTask.Result && !httpTask.Result
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask.Result.GetMessage()}" )
            : IReply.Success();
    }
    public async Task<Reply<bool>> Update( CartItemDto item, bool authenticated )
    {
        var cartReply = await Get( authenticated );
        if (!cartReply)
            return IReply.Fail( cartReply );
        
        SetBusy( true );
        CartSummary summary = cartReply.Data;
        if (!summary.Contains( item.ProductId ))
            return IReply.Conflict( "Cart does not contain this item." );
        summary.Set( item );
        
        var storageTask = _storage.SetLocalStorage( SummaryStorageKey, summary );
        var httpTask = authenticated ? _http.PutAsyncAuthenticated<bool>( Consts.ApiUpdateCart, item ) : null;

        if (httpTask is null) await storageTask;
        else await Task.WhenAll( storageTask, httpTask );
        
        _summaryInMemory = summary;
        SetBusy( false );

        return !storageTask.Result && (!authenticated || !httpTask!.Result)
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask?.Result.GetMessage()}" )
            : IReply.Success();
    }
    public async Task<Reply<bool>> Delete( Guid productId, bool authenticated )
    {
        var cartReply = await Get( authenticated );
        if (!cartReply)
            return IReply.Fail( cartReply );
        
        SetBusy( true );
        CartSummary summary = cartReply.Data;
        if (!summary.Contains( productId ))
            return IReply.Conflict( "Cart does not contain this item." );
        summary.Delete( productId );

        var storageTask = _storage.SetLocalStorage( SummaryStorageKey, summary );
        var httpTask = authenticated
            ? _http.DeleteAsyncAuthenticated<bool>( Consts.ApiDeleteFromCart,
                new Dictionary<string, object>() { { "ProductId", productId } } )
            : null;
        
        if (httpTask is null) await storageTask;
        else await Task.WhenAll( storageTask, httpTask );
        
        _summaryInMemory = summary;
        SetBusy( false );

        return !storageTask.Result && (!authenticated || !httpTask!.Result)
            ? IReply.Fail( $"Failed to update cart in storage or server. {storageTask.Result.GetMessage()} {httpTask?.Result.GetMessage()}" )
            : IReply.Success();
    }
    
    async Task<bool> IsBusy()
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
    void SetBusy( bool value )
    {
        lock ( _lock )
            _isBusy = value;
    }
}