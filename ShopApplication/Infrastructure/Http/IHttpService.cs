using RetailDomain.Optionals;

namespace ShopApplication.Infrastructure.Http;

public interface IHttpService
{
    public Task<Opt<T>> TryGetRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null );
    public Task<Opt<T>> TryPostRequest<T>( string apiPath, object? body = null, string? authToken = null );
    public Task<Opt<T>> TryPutRequest<T>( string apiPath, object? body = null, string? authToken = null );
    public Task<Opt<T>> TryDeleteRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null );
}