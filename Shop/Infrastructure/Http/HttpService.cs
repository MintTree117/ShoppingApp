using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using Shop.Infrastructure.Authentication;
using Shop.Infrastructure.Common.ReplyTypes;
using Shop.Utilities;

namespace Shop.Infrastructure.Http;

public sealed class HttpService( IHttpClientFactory httpFactory, IServiceProvider provider ) // SINGLETON
{
    readonly IHttpClientFactory _httpFactory = httpFactory;
    readonly IServiceProvider _provider = provider;
    readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
    
    HttpClient CreateScopedClient( bool authenticate )
    {
        HttpClient client = _httpFactory.CreateClient( "API" );

        if (!authenticate)
            return client;
        // Enable sending credentials (cookies) with requests
        client.DefaultRequestHeaders.Add( "X-Requested-With", "XMLHttpRequest" );

        // Ensure HttpClient includes credentials
        AuthenticationStateManager auth = _provider.GetService<AuthenticationStateManager>() ?? throw new Exception( "HttpService: Failed to get SessionManager from Provider." );
        client.SetAuthenticationHeader( auth.AccessToken );
        return client;
    }
    
    public async Task<Reply<T>> GetAsync<T>( string url, Dictionary<string, object>? parameters = null ) =>
        await ExecuteGetRequest<T>( url, false, parameters );
    public async Task<Reply<T>> PostAsync<T>( string url, object? body = null ) =>
        await ExecutePostRequest<T>( url, false, body );
    public async Task<Reply<T>> PutAsync<T>( string url, object? body = null ) =>
        await ExecutePutRequest<T>( url, false, body );
    public async Task<Reply<T>> DeleteAsync<T>( string url, Dictionary<string, object>? parameters = null ) =>
        await ExecuteDeleteRequest<T>( url, false, parameters );
    
    public async Task<Reply<T>> GetAsyncAuthenticated<T>( string url, Dictionary<string, object>? parameters = null ) => 
        await ExecuteGetRequest<T>( url, true, parameters );
    public async Task<Reply<T>> PostAsyncAuthenticated<T>( string url, object? body = null ) =>
        await ExecutePostRequest<T>( url, true, body );
    public async Task<Reply<T>> PutAsyncAuthenticated<T>( string url, object? body = null ) =>
        await ExecutePutRequest<T>( url, true, body );
    public async Task<Reply<T>> DeleteAsyncAuthenticated<T>( string url, Dictionary<string, object>? parameters = null ) =>
        await ExecuteDeleteRequest<T>( url, true, parameters );
    
    async Task<Reply<T>> ExecuteGetRequest<T>( string url, bool authenticate, Dictionary<string, object>? parameters = null )
    {
        string urlWithParams = BuildQueryUrl( url, parameters );
        
        try 
        {
            using HttpClient client = CreateScopedClient( authenticate );
            HttpResponseMessage response = await client.GetAsync( urlWithParams );
            return await ParseHttpResponse<T>( response );
        }
        catch ( Exception e ) 
        {
            return HandleHttpException<T>( e, "GET", urlWithParams );
        }
    }
    async Task<Reply<T>> ExecutePostRequest<T>( string url, bool authenticate, object? body = null )
    {
        try
        {
            using HttpClient client = CreateScopedClient( authenticate );
            HttpResponseMessage response = await client.PostAsJsonAsync( url, body );
            return await ParseHttpResponse<T>( response );
        }
        catch ( Exception e ) 
        {
            return HandleHttpException<T>( e, "POST", url );
        }
    }
    async Task<Reply<T>> ExecutePutRequest<T>( string url, bool authenticate, object? body = null )
    {
        try
        {
            using HttpClient client = CreateScopedClient( authenticate );
            HttpResponseMessage response = await client.PutAsJsonAsync( url, body );
            return await ParseHttpResponse<T>( response );
        }
        catch ( Exception e ) 
        {
            return HandleHttpException<T>( e, "PUT", url );
        }
    }
    async Task<Reply<T>> ExecuteDeleteRequest<T>( string url, bool authenticate, Dictionary<string, object>? parameters = null )
    {
        string urlWithParams = BuildQueryUrl( url, parameters );
        
        try
        {
            using HttpClient client = CreateScopedClient( authenticate );
            HttpResponseMessage response = await client.DeleteAsync( urlWithParams );
            return await ParseHttpResponse<T>( response );
        }
        catch ( Exception e ) 
        {
            return HandleHttpException<T>( e, "DELETE", urlWithParams );
        }
    }
    
    static string BuildQueryUrl( string apiPath, Dictionary<string, object>? parameters )
    {
        if (parameters is null)
            return apiPath;

        NameValueCollection query = HttpUtility.ParseQueryString( string.Empty );
        foreach ( KeyValuePair<string, object> param in parameters ) 
            query[param.Key] = param.Value.ToString();

        return $"{apiPath}?{query}";
    }
    async Task<Reply<T>> ParseHttpResponse<T>( HttpResponseMessage httpResponse )
    {
        if (!httpResponse.IsSuccessStatusCode)
            return LogReturn<T>( httpResponse.StatusCode + await httpResponse.Content.ReadAsStringAsync() );
        
        var httpContent = await httpResponse.Content.ReadFromJsonAsync<T>();
        return httpContent is not null
            ? Reply<T>.Success( httpContent )
            : Reply<T>.NetworkError();
    }
    static Reply<T> LogReturn<T>( string message )
    {
        Logger.Log( $"Http response error: {message}" );
        return Reply<T>.NetworkError();
    }
    static Reply<T> HandleHttpException<T>( Exception e, string requestType, string requestUrl )
    {
        Logger.LogError( e, $"Http {requestType} Exception.", requestUrl );
        return Reply<T>.NetworkError();
    }
}