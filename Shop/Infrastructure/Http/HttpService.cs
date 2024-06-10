using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Web;
using Shop.Infrastructure.Common.Optionals;
using Shop.Utilities;

namespace Shop.Infrastructure.Http;

public sealed class HttpService( IHttpClientFactory httpFactory )
{
    readonly IHttpClientFactory _httpFactory = httpFactory;

    HttpClient CreateClient( string? authentication )
    {
        HttpClient http = _httpFactory.CreateClient();
        http.SetAuthenticationHeader( authentication );
        return http;
    }
    
    public async Task<Reply<T>> TryGetRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null )
    {
        try {
            string path = ConstructHttpQuery( apiPath, parameters );
            HttpResponseMessage httpResponse = await CreateClient( authToken ).GetAsync( path );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpException<T>( e, "GET", apiPath );
        }
    }
    public async Task<Reply<T>> TryPostRequest<T>( string apiPath, object? body = null, string? authToken = null )
    {
        try {
            HttpResponseMessage httpResponse = await CreateClient( authToken ).PostAsJsonAsync( apiPath, body );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpException<T>( e, "POST", apiPath );
        }
    }
    public async Task<Reply<T>> TryPutRequest<T>( string apiPath, object? body = null, string? authToken = null )
    {
        try {
            HttpResponseMessage httpResponse = await CreateClient( authToken ).PutAsJsonAsync( apiPath, body );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpException<T>( e, "PUT", apiPath );
        }
    }
    public async Task<Reply<T>> TryDeleteRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null )
    {
        try {
            string path = ConstructHttpQuery( apiPath, parameters );
            HttpResponseMessage httpResponse = await CreateClient( authToken ).DeleteAsync( path );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpException<T>( e, "DELETE", apiPath );
        }
    }
    
    static string ConstructHttpQuery( string apiPath, Dictionary<string, object>? parameters )
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
                ? Reply<T>.With( httpContent )
                : Reply<T>.None( "No data returned from http request." );
        }

        string errorContent = await httpResponse.Content.ReadAsStringAsync();
        return LogErrorAndReturn<T>( httpResponse.StatusCode + errorContent );
    }
    static Reply<T> LogErrorAndReturn<T>( string message )
    {
        Console.WriteLine( $"An error occured during an http request : {message}" );
        return Reply<T>.None( $"An error occured during an http request : {message}" );
    }
    static Reply<T> HandleHttpException<T>( Exception e, string requestType, string requestUrl )
    {
        Logger.LogError( $"HTTP {requestType} ERROR", requestUrl, e );
        return Reply<T>.Exception( e, "An error occured during a network request." );
    }
}