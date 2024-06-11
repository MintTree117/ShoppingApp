using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Web;
using Shop.Infrastructure.Authentication;
using Shop.Infrastructure.Common.ReplyTypes;
using Shop.Utilities;

namespace Shop.Infrastructure.Http;

public sealed class HttpService( IHttpClientFactory httpFactory, AuthenticationService authService ) // SINGLETON
{
    readonly IHttpClientFactory _httpFactory = httpFactory;
    readonly AuthenticationService _authService = authService;
    
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
        string urlWithParams = ConstructQueryWithParams( url, parameters );
        
        try 
        {
            using HttpClient client = await CreateScopedClient( authenticate );
            HttpResponseMessage response = await client.GetAsync( urlWithParams );
            return await HandleHttpResponse<T>( response );
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
            using HttpClient client = await CreateScopedClient( authenticate );
            HttpResponseMessage response = await client.PostAsJsonAsync( url, body );
            return await HandleHttpResponse<T>( response );
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
            using HttpClient client = await CreateScopedClient( authenticate );
            HttpResponseMessage response = await client.PutAsJsonAsync( url, body );
            return await HandleHttpResponse<T>( response );
        }
        catch ( Exception e ) 
        {
            return HandleHttpException<T>( e, "PUT", url );
        }
    }
    async Task<Reply<T>> ExecuteDeleteRequest<T>( string url, bool authenticate, Dictionary<string, object>? parameters = null )
    {
        string urlWithParams = ConstructQueryWithParams( url, parameters );
        
        try
        {
            using HttpClient client = await CreateScopedClient( authenticate );
            HttpResponseMessage response = await client.DeleteAsync( urlWithParams );
            return await HandleHttpResponse<T>( response );
        }
        catch ( Exception e ) 
        {
            return HandleHttpException<T>( e, "DELETE", urlWithParams );
        }
    }
    
    async Task<HttpClient> CreateScopedClient( bool authenticate )
    {
        HttpClient http = _httpFactory.CreateClient();
        string? header = authenticate ? await _authService.AccessToken() : null;
        http.SetAuthenticationHeader( header );
        return http;
    }
    
    static string ConstructQueryWithParams( string apiPath, Dictionary<string, object>? parameters )
    {
        if (parameters is null)
            return apiPath;

        NameValueCollection query = HttpUtility.ParseQueryString( string.Empty );

        foreach ( KeyValuePair<string, object> param in parameters ) query[param.Key] = param.Value.ToString();

        return $"{apiPath}?{query}";
    }
    static async Task<Reply<T>> HandleHttpResponse<T>( HttpResponseMessage httpResponse )
    {
        if (httpResponse.IsSuccessStatusCode) {
            T? httpContent = await httpResponse.Content.ReadFromJsonAsync<T>();
            return httpContent is not null
                ? Reply<T>.True( httpContent )
                : Reply<T>.False( "No data returned from http request." );
        }

        string errorContent = await httpResponse.Content.ReadAsStringAsync();
        return LogErrorAndReturn<T>( httpResponse.StatusCode + errorContent );
    }
    static Reply<T> LogErrorAndReturn<T>( string message )
    {
        Console.WriteLine( $"An error occured during an http request : {message}" );
        return Reply<T>.False( $"An error occured during an http request : {message}" );
    }
    static Reply<T> HandleHttpException<T>( Exception e, string requestType, string requestUrl )
    {
        Logger.LogError( e, $"HTTP {requestType} ERROR", requestUrl );
        return Reply<T>.False( e, "An error occured during a network request." );
    }
}