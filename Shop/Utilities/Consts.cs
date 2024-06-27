namespace Shop.Utilities;

internal static class Consts
{
    // API
    // ----------------------------------------------------------------------------------------------------------------
    // BASE
    internal const string OrderingApiBase = "https://localhost:7212/api";
    
    // ACCOUNT REGISTRATION
    internal const string ApiRegister = OrderingApiBase + "/account/register";
    internal const string ApiConfirmEmail = OrderingApiBase + "/account/register/confirmEmail";
    internal const string ApiResendConfirm = OrderingApiBase + "/account/register/resendConfirmEmail";
    
    // AUTHENTICATION
    internal const string ApiLogin = OrderingApiBase + "/authentication/login";
    internal const string ApiTwoFactor = OrderingApiBase + "/authentication/2fa";
    internal const string ApiLoginRecovery = OrderingApiBase + "/authentication/recover";
    internal const string ApiLoginRefresh = OrderingApiBase + "/authentication/refresh";
    internal const string ApiForgotPassword = OrderingApiBase + "/authentication/forgot";
    internal const string ApiResetPassword = OrderingApiBase + "/authentication/reset";
    internal const string ApiLogout = OrderingApiBase + "/authentication/logout";
    
    // ACCOUNT PROFILE
    internal const string ApiGetAccountProfile = OrderingApiBase + "/account/profile/view";
    internal const string ApiUpdateAccountProfile = OrderingApiBase + "/account/profile/update";

    // ACCOUNT SECURITY
    internal const string ApiViewSecurity = OrderingApiBase + "/account/security/view";
    internal const string ApiUpdatePassword = OrderingApiBase + "/account/security/updatePassword";
    internal const string ApiGenerateRecoveryCodes = OrderingApiBase + "/account/security/generateRecovery";
    internal const string ApiDisable2Fa = OrderingApiBase + "/account/security/disable2fa";
    internal const string ApiUpdate2Fa = OrderingApiBase + "/account/security/update2fa";

    // ACCOUNT ADDRESSES
    internal const string ApiGetAddresses = OrderingApiBase + "/account/addresses/view";
    internal const string ApiAddAddress = OrderingApiBase + "/account/addresses/add";
    internal const string ApiUpdateAddress = OrderingApiBase + "/account/addresses/update";
    internal const string ApiDeleteAddress = OrderingApiBase + "/account/addresses/delete";

    // DELETE ACCOUNT
    internal const string ApiDeleteAccount = OrderingApiBase + "/account/delete";
    
    // CATALOG
    internal const string CatalogApiBase = "https://localhost:7123/api";
    internal const string ApiGetCategories = CatalogApiBase + "/categories";
    internal const string ApiGetBrands = CatalogApiBase + "/brands";
    internal const string ApiGetSuggestions = CatalogApiBase + "/suggestions";
    internal const string ApiGetSearch = CatalogApiBase + "/search";
    internal const string ApiGetEstimates = CatalogApiBase + "/estimates";
    internal const string ApiGetDetails = CatalogApiBase + "/details";
    internal const string ApiGetProductsByIds = CatalogApiBase + "/view";
    internal const string ApiGetSpecials = CatalogApiBase + "/specials";
    
    // ORDERING
    internal const string ApiCartBase = "/cart";
    internal const string ApiPostGetCart = ApiCartBase + "/postGet";
    internal const string ApiAddToCart = ApiCartBase + "/add";
    internal const string ApiUpdateCart = ApiCartBase + "/update";
    internal const string ApiUpdateCartBulk = ApiCartBase + "/updateBulk";
    internal const string ApiDeleteFromCart = ApiCartBase + "/delete";
    internal const string ApiClearCart = ApiCartBase + "/clear";
    
    // PARAMS
    // ----------------------------------------------------------------------------------------------------------------
    internal const string ParamReturnUrl = "ReturnUrl";

    // PAGES
    // ----------------------------------------------------------------------------------------------------------------
    // HOME
    internal const string PageHome = "/";
    // AUTHENTICATION
    internal const string PageEmailConfirm = "/confirmEmail";
    internal const string PageEmailConfirmResend = "/resendEmailConfirmation";
    internal const string PageLoginOrRegister = "/loginOrRegister";
    internal const string PageLoginRecovery = "/loginRecovery";
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
    internal const string PageAccountProfile = PageAccountBase + "/profile";
    internal const string PageAccountSecurity = PageAccountBase + "/security";
    // CATALOG
    internal const string PageProductDetails = "/product";
    internal const string PageProductSearch = "/search";
    // ORDERING
    internal const string PageCart = "/cart";
    internal const string PageCheckout = "/checkout";
}
