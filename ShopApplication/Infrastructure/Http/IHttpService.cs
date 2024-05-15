using ShopApplication.Types.Optionals;

namespace ShopApplication.Infrastructure.Http;

public interface IHttpService
{
    public Task<OptObj<T>> TryGetObjRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : class, new();
    public Task<OptObj<T>> TryPostObjRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : class, new();
    public Task<OptObj<T>> TryPutObjRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : class, new();
    public Task<OptObj<T>> TryDeleteObjRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : class, new();
    
    public Task<OptVal<T>> TryGetValRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : struct;
    public Task<OptVal<T>> TryPostValRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : struct;
    public Task<OptVal<T>> TryPutValRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : struct;
    public Task<OptVal<T>> TryDeleteValRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : struct;
}