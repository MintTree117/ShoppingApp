namespace Shop.Utilities;

internal static class Consts
{
    // API
    // ----------------------------------------------------------------------------------------------------------------
    // BASE
    internal const string ApiBase = "https://localhost:7212/api";
    
    // ACCOUNT ADDRESSES
    internal const string ApiGetAddresses = ApiBase + "/account/addresses/view";
    internal const string ApiAddAddress = ApiBase + "/account/addresses/add";
    internal const string ApiUpdateAddress = ApiBase + "/account/addresses/update";
    internal const string ApiDeleteAddress = ApiBase + "/account/addresses/delete";

    // AUTHENTICATION
    internal const string ApiLogin = ApiBase + "/authentication/login";
    internal const string ApiTwoFactor = ApiBase + "/authentication/2fa";
    internal const string ApiLoginRecovery = ApiBase + "/authentication/recover";
    internal const string ApiLoginRefresh = ApiBase + "/authentication/refresh";
    internal const string ApiForgotPassword = ApiBase + "/authentication/forgot";
    internal const string ApiResetPassword = ApiBase + "/authentication/reset";
    internal const string ApiLogout = ApiBase + "/authentication/logout";
    
    // DELETE ACCOUNT
    internal const string ApiDeleteAccount = ApiBase + "/account/delete";
    
    // ACCOUNT PROFILE
    internal const string ApiGetAccountProfile = ApiBase + "/account/profile/view";
    internal const string ApiUpdateAccountProfile = ApiBase + "/account/profile/update";
    
    // ACCOUNT REGISTRATION
    internal const string ApiRegister = ApiBase + "/account/register";
    internal const string ApiConfirmEmail = ApiBase + "/account/register/confirmEmail";
    internal const string ApiResendConfirm = ApiBase + "/account/register/resendConfirmEmail";

    // ACCOUNT SECURITY
    internal const string ApiViewSecurity = ApiBase + "/account/security/view";
    internal const string ApiUpdatePassword = ApiBase + "/account/security/updatePassword";
    internal const string ApiDisable2Fa = ApiBase + "/account/security/disable2fa";
    internal const string ApiUpdate2Fa = ApiBase + "/account/security/update2fa";
    
    
    // CATALOG
    internal const string ApiGetCategories = ApiBase + "/categories";
    internal const string ApiGetBrands = ApiBase + "/brands";
    
    // PARAMS
    // ----------------------------------------------------------------------------------------------------------------
    internal const string ParamReturnUrl = "ReturnUrl";

    // PAGES
    // ----------------------------------------------------------------------------------------------------------------
    // HOME
    internal const string PageHome = "/";
    // AUTHENTICATION
    internal const string PageEmailConfirm = "/emailConfirm";
    internal const string PageEmailConfirmResend = "/emailResend";
    internal const string PageLoginOrRegister = "/loginOrRegister";
    internal const string PageLogout = "/logout";
    internal const string PageForgotPassword = "/passwordForgot";
    internal const string PageResetPassword = "/passwordReset";
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
}
