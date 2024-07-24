namespace Shop.Utilities;

internal static class Consts
{
    internal const int DelayTime = 250;
    
    // API
    // ----------------------------------------------------------------------------------------------------------------
    // ACCOUNT REGISTRATION
    internal const string ApiRegister = "/account/register";
    internal const string ApiConfirmEmail = "/account/register/confirmEmail";
    internal const string ApiResendConfirm = "/account/register/resendConfirmEmail";
    
    // AUTHENTICATION
    internal const string ApiLogin = "/authentication/login";
    internal const string ApiTwoFactor = "/authentication/2fa";
    internal const string ApiLoginRecovery = "/authentication/recover";
    internal const string ApiLoginRefresh = "/authentication/refresh";
    internal const string ApiForgotPassword = "/authentication/forgot";
    internal const string ApiResetPassword = "/authentication/reset";
    internal const string ApiLogout = "/authentication/logout";
    
    // ACCOUNT PROFILE
    internal const string ApiGetAccountProfile = "/account/profile/view";
    internal const string ApiUpdateAccountProfile = "/account/profile/update";
    
    // ACCOUNT SECURITY
    internal const string ApiViewSecurity = "/account/security/view";
    internal const string ApiUpdatePassword = "/account/security/updatePassword";
    internal const string ApiGenerateRecoveryCodes = "/account/security/generateRecovery";
    internal const string ApiDisable2Fa = "/account/security/disable2fa";
    internal const string ApiUpdate2Fa = "/account/security/update2fa";
    
    // ACCOUNT ORDERS
    internal const string ApiOrdersView = "/account/orders/view";
    internal const string ApiOrdersDetails = "/account/orders/details";
    
    // ACCOUNT ADDRESSES
    internal const string ApiGetAddresses = "/account/addresses/view";
    internal const string ApiAddAddress = "/account/addresses/add";
    internal const string ApiUpdateAddress = "/account/addresses/update";
    internal const string ApiDeleteAddress = "/account/addresses/delete";

    // ACCOUNT SESSIONS
    internal const string ApiGetSessions = "/account/sessions/view";
    internal const string ApiDeleteSession = "/account/sessions/delete";
    
    // DELETE ACCOUNT
    internal const string ApiDeleteAccount = "/account/delete";

    // CART
    internal const string ApiPostGetCart = "/cart/postGet";
    internal const string ApiClearCart = "/cart/clear";
    
    // ORDERING
    internal const string ApiPlaceOrderGuest = "/orders/place/guest";
    internal const string ApiPlaceOrderUser = "/orders/place/user";
    
    // CATALOG
    internal const string ApiGetCategories = "/categories";
    internal const string ApiGetBrands = "/brands";
    internal const string ApiGetSpecials = "/specials";
    internal const string ApiGetSuggestions = "/searchSuggestions";
    internal const string ApiGetProductsByIds = "/searchIds";
    internal const string ApiGetSimilarProducts = "/searchSimilar";
    internal const string ApiGetEstimates = "/shippingEstimates";
    internal const string ApiGetSearch = "/searchFull";
    internal const string ApiGetDetails = "/productDetails";
    
    // PARAMS
    // ----------------------------------------------------------------------------------------------------------------
    internal const string ParamReturnUrl = "ReturnUrl";
    internal const string DefaultSearchParams = "Page=1&PageSize=5&SortBy=0";
    internal static string GetProductSearchUrl( string parameters ) =>
        $"{PageProductSearch}{parameters}{DefaultSearchParams}";
    internal static string GetProductDetailsUrl( Guid productId ) =>
        $"{PageProductDetails}?ProductId={productId}";
    internal static string GetProductDetailsUrlRatings( Guid productId ) =>
        $"{PageProductDetails}?ProductId={productId}&ShowRatings";
    internal static string GetBrandSearchUrl( Guid brandId ) =>
        GetProductSearchUrl( $"?BrandIds={brandId}&" );


    // PAGES
    // ----------------------------------------------------------------------------------------------------------------
    // HOME
    internal const string PageHome = "/";
    // AUTHENTICATION
    internal const string PageEmailConfirm = "/confirmEmail";
    internal const string PageEmailConfirmResend = "/resendEmailConfirmation";
    internal const string PageLoginOrRegister = "/loginOrRegister";
    internal const string RecoveryLogin = "/loginRecovery";
    internal const string PageLogout = "/logout";
    internal const string PageForgotPassword = "/forgotPassword";
    internal const string PageResetPassword = "/resetPassword";
    internal const string PageSessionExpired = "/sessionExpired";
    // ACCOUNT
    internal const string PageAccountBase = "/account";
    internal const string PageAccountAddresses = PageAccountBase + "/addresses";
    internal const string PageAccountDelete = PageAccountBase + "/delete";
    internal const string PageAccountDeleted = PageAccountBase + "/deleted";
    internal const string PageAccountOrders = PageAccountBase + "/orders";
    internal const string PageAccountSessions = PageAccountBase + "/sessions";
    internal const string PageAccountProfile = PageAccountBase + "/profile";
    internal const string PageAccountSecurity = PageAccountBase + "/security";
    // CATALOG
    internal const string PageProductDetails = "/product";
    internal const string PageProductSearch = "/search";
    // ORDERING
    internal const string PageCart = "/cart";
    internal const string PageCheckout = "/checkout";
    internal const string PageOrderPlaced = "/orderPlaced";
    // SUPPORT
    internal const string PageSupport = "/support";
}
