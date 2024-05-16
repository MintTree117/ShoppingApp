using ShopApplication.Common.Optionals;

namespace ShopApplication.Infrastructure.Http;

public interface IHttpService
{
    public Task<Obj<T>> TryGetObjRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : class, new();
    public Task<Obj<T>> TryPostObjRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : class, new();
    public Task<Obj<T>> TryPutObjRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : class, new();
    public Task<Obj<T>> TryDeleteObjRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : class, new();
    
    public Task<Val<T>> TryGetValRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : struct;
    public Task<Val<T>> TryPostValRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : struct;
    public Task<Val<T>> TryPutValRequest<T>( string apiPath, object? body = null, string? authToken = null ) where T : struct;
    public Task<Val<T>> TryDeleteValRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null ) where T : struct;
}