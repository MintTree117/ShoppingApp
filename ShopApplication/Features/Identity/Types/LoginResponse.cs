namespace ShopApplication.Features.Identity.Types;

public readonly record struct LoginResponse(
    string JwtToken );