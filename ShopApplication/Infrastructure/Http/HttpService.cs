using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Web;
using RetailDomain.Optionals;

namespace ShopApplication.Infrastructure.Http;

internal sealed class HttpService( IHttpClientFactory httpFactory ) : IHttpService
{
    readonly IHttpClientFactory _httpFactory = httpFactory;

    HttpClient CreateClient( string? authentication )
    {
        HttpClient http = _httpFactory.CreateClient();
        http.SetAuthenticationHeader( authentication );
        return http;
    }
    
    public async Task<Opt<T>> TryGetRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null )
    {
        try {
            string path = ConstructHttpQuery( apiPath, parameters );
            HttpResponseMessage httpResponse = await CreateClient( authToken ).GetAsync( path );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpException<T>( e, "Get", apiPath );
        }
    }
    public async Task<Opt<T>> TryPostRequest<T>( string apiPath, object? body = null, string? authToken = null )
    {
        try {
            HttpResponseMessage httpResponse = await CreateClient( authToken ).PostAsJsonAsync( apiPath, body );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpException<T>( e, "Post", apiPath );
        }
    }
    public async Task<Opt<T>> TryPutRequest<T>( string apiPath, object? body = null, string? authToken = null )
    {
        try {
            HttpResponseMessage httpResponse = await CreateClient( authToken ).PutAsJsonAsync( apiPath, body );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpException<T>( e, "Put", apiPath );
        }
    }
    public async Task<Opt<T>> TryDeleteRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null )
    {
        try {
            string path = ConstructHttpQuery( apiPath, parameters );
            HttpResponseMessage httpResponse = await CreateClient( authToken ).DeleteAsync( path );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpException<T>( e, "Delete", apiPath );
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
    static async Task<Opt<T>> HandleHttpResponse<T>( HttpResponseMessage httpResponse )
    {
        if (httpResponse.IsSuccessStatusCode) {
            var httpContent = await httpResponse.Content.ReadFromJsonAsync<T>();
            return httpContent is not null
                ? Opt<T>.With( httpContent )
                : Opt<T>.None( "No data returned from http request." );
        }

        string errorContent = await httpResponse.Content.ReadAsStringAsync();
        return LogErrorAndReturn<T>( errorContent );
    }
    static Opt<T> LogErrorAndReturn<T>( string message )
    {
        Console.WriteLine( $"An exception was thrown during an http request : {message}" );
        return Opt<T>.None( $"An exception was thrown during an http request : {message}" );
    }
    static Opt<T> HandleHttpException<T>( Exception e, string requestType, string requestUrl )
    {
        Console.WriteLine( e );
        return Opt<T>.Exception( e, $"{requestType}: An exception occured while executing http: {requestUrl}" );
    }
}