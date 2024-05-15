using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Web;
using ShoppingApplication.Types;
using ShoppingApplication.Types.Optionals;

namespace ShoppingApplication.Infrastructure.Http;

internal sealed class HttpService( IHttpClientFactory httpFactory )
{
    readonly IHttpClientFactory _httpFactory = httpFactory;

    HttpClient CreateClient( string? authentication )
    {
        HttpClient http = _httpFactory.CreateClient();
        http.SetAuthenticationHeader( authentication );
        return http;
    }
    
    public async Task<OptObj<T>> TryGetObjRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : class, new()
    {
        try {
            string path = ConstructQuery( apiPath, parameters );
            HttpResponseMessage httpResponse = await CreateClient( authToken ).GetAsync( path );
            return await HandleHttpObjResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpObjException<T>( e, "Get", apiPath );
        }
    }
    public async Task<OptObj<T>> TryPostObjRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : class, new()
    {
        try {
            HttpResponseMessage httpResponse = await CreateClient( authToken ).PostAsJsonAsync( apiPath, body );
            return await HandleHttpObjResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpObjException<T>( e, "Post", apiPath );
        }
    }
    public async Task<OptObj<T>> TryPutObjRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : class, new()
    {
        try {
            HttpResponseMessage httpResponse = await CreateClient( authToken ).PutAsJsonAsync( apiPath, body );
            return await HandleHttpObjResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpObjException<T>( e, "Put", apiPath );
        }
    }
    public async Task<OptObj<T>> TryDeleteObjRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : class, new()
    {
        try {
            string path = ConstructQuery( apiPath, parameters );
            HttpResponseMessage httpResponse = await CreateClient( authToken ).DeleteAsync( path );
            return await HandleHttpObjResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpObjException<T>( e, "Delete", apiPath );
        }
    }

    public async Task<OptVal<T>> TryGetValRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : struct
    {
        try {
            string path = ConstructQuery( apiPath, parameters );
            HttpResponseMessage httpResponse = await CreateClient( authToken ).GetAsync( path );
            return await HandleHttpValResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpValException<T>( e, "Get", apiPath );
        }
    }
    public async Task<OptVal<T>> TryPostValRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : struct
    {
        try {
            HttpResponseMessage httpResponse = await CreateClient( authToken ).PostAsJsonAsync( apiPath, body );
            return await HandleHttpValResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpValException<T>( e, "Post", apiPath );
        }
    }
    public async Task<OptVal<T>> TryPutValRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : struct
    {
        try {
            HttpResponseMessage httpResponse = await CreateClient( authToken ).PutAsJsonAsync( apiPath, body );
            return await HandleHttpValResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpValException<T>( e, "Put", apiPath );
        }
    }
    public async Task<OptVal<T>> TryDeleteValRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : struct
    {
        try {
            string path = ConstructQuery( apiPath, parameters );
            HttpResponseMessage httpResponse = await CreateClient( authToken ).DeleteAsync( path );
            return await HandleHttpValResponse<T>( httpResponse );
        }
        catch ( Exception e ) {
            return HandleHttpValException<T>( e, "Delete", apiPath );
        }
    }
    
    static string ConstructQuery( string apiPath, Dictionary<string, object>? parameters )
    {
        if (parameters is null)
            return apiPath;

        NameValueCollection query = HttpUtility.ParseQueryString( string.Empty );

        foreach ( KeyValuePair<string, object> param in parameters ) query[param.Key] = param.Value.ToString();

        return $"{apiPath}?{query}";
    }
    static async Task<OptObj<T>> HandleHttpObjResponse<T>( HttpResponseMessage httpResponse ) where T : class, new()
    {
        if (httpResponse.IsSuccessStatusCode) {
            var httpContent = await httpResponse.Content.ReadFromJsonAsync<T>();
            return httpContent is not null
                ? OptObj<T>.Success( httpContent )
                : OptObj<T>.Failure( Problem.Network, "No data returned from http request." );
        }

        string errorContent = await httpResponse.Content.ReadAsStringAsync();
        return httpResponse.StatusCode switch {
            HttpStatusCode.BadRequest => LogObjErrorAndReturn<T>( Problem.BadRequest, errorContent ),
            HttpStatusCode.NotFound => LogObjErrorAndReturn<T>( Problem.NotFound, errorContent ),
            HttpStatusCode.Unauthorized => LogObjErrorAndReturn<T>( Problem.Unauthorized, errorContent ),
            HttpStatusCode.Conflict => LogObjErrorAndReturn<T>( Problem.Conflict, errorContent ),
            HttpStatusCode.InternalServerError => LogObjErrorAndReturn<T>( Problem.Internal, errorContent ),
            _ => LogObjErrorAndReturn<T>( Problem.Internal, errorContent )
        };
    }
    static async Task<OptVal<T>> HandleHttpValResponse<T>( HttpResponseMessage httpResponse ) where T : struct
    {
        if (httpResponse.IsSuccessStatusCode)
            return OptVal<T>.Success( await httpResponse.Content.ReadFromJsonAsync<T>() );

        string errorContent = await httpResponse.Content.ReadAsStringAsync();
        return httpResponse.StatusCode switch {
            HttpStatusCode.BadRequest => LogValErrorAndReturn<T>( Problem.BadRequest, errorContent ),
            HttpStatusCode.NotFound => LogValErrorAndReturn<T>( Problem.NotFound, errorContent ),
            HttpStatusCode.Unauthorized => LogValErrorAndReturn<T>( Problem.Unauthorized, errorContent ),
            HttpStatusCode.Conflict => LogValErrorAndReturn<T>( Problem.Conflict, errorContent ),
            HttpStatusCode.InternalServerError => LogValErrorAndReturn<T>( Problem.Internal, errorContent ),
            _ => LogValErrorAndReturn<T>( Problem.Internal, errorContent )
        };
    }
    static OptObj<T> LogObjErrorAndReturn<T>( Problem error, string message ) where T : class, new()
    {
        Console.WriteLine( $"{error} : An exception was thrown during an http request : {message}" );
        return OptObj<T>.Failure( error, $"An exception was thrown during an http request : {message}" );
    }
    static OptVal<T> LogValErrorAndReturn<T>( Problem error, string message ) where T : struct
    {
        Console.WriteLine( $"{error} : An exception was thrown during an http request : {message}" );
        return OptVal<T>.Failure( error, $"An exception was thrown during an http request : {message}" );
    }
    static OptObj<T> HandleHttpObjException<T>( Exception e, string requestType, string requestUrl ) where T : class, new()
    {
        Console.WriteLine( e );
        return OptObj<T>.Exception( e, Problem.Internal, $"{requestType}: An exception occured while executing http: {requestUrl}" );
    }
    static OptVal<T> HandleHttpValException<T>( Exception e, string requestType, string requestUrl ) where T : struct
    {
        Console.WriteLine( e );
        return OptVal<T>.Exception( e, Problem.Internal, $"{requestType}: An exception occured while executing http: {requestUrl}" );
    }
}