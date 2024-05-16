namespace ShopApplication.Types.Identity;

public readonly record struct LoginResponse(
    string JwtToken );