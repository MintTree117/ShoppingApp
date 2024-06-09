namespace Shop.Utilities;

internal static class Consts
{
    // API
    // ----------------------------------------------------------------------------------------------------------------
    // BASE
    internal const string ApiBase = "https://localhost:7212/api";
    // AUTHENTICATION
    internal const string ApiIdentityBase = ApiBase + "/identity";
    internal const string ApiRegister = ApiIdentityBase + "/register";
    internal const string ApiConfirmEmail = ApiIdentityBase + "/email/confirm";
    internal const string ApiResendConfirm = ApiIdentityBase + "/email/resend";
    internal const string ApiLoginCheck = ApiIdentityBase + "/check";
    internal const string ApiLogout = ApiIdentityBase + "/logout";
    internal const string ApiLogin = ApiIdentityBase + "/login";
    internal const string ApiTwoFactor = ApiIdentityBase + "/2fa";
    internal const string ApiLoginRefresh = ApiIdentityBase + "/refresh";
    internal const string ApiLoginRefreshFull = ApiIdentityBase + "/refresh-full";
    internal const string ApiForgotPassword = ApiIdentityBase + "/password/forgot";
    internal const string ApiResetPassword = ApiIdentityBase + "/password/reset";
    // ACCOUNT
    internal const string ApiAccountBase = ApiBase + "/account";
    internal const string ApiUpdatePassword = ApiAccountBase + "/password/update";
    internal const string ApiGetAccountDetails = ApiAccountBase + "/account/view";
    internal const string ApiUpdateAccountDetails = ApiAccountBase + "/account/update";
    internal const string ApiDeleteAccount = ApiAccountBase + "/account/delete";
    internal const string ApiGetAddresses = ApiAccountBase + "/address/view";
    internal const string ApiAddAddress = ApiAccountBase + "/address/add";
    internal const string ApiUpdateAddress = ApiAccountBase + "/address/update";
    internal const string ApiDeleteAddress = ApiAccountBase + "/address/delete";
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
    internal const string PageProductDetails = "/productDetails";
    internal const string PageProductSearch = "/productSearch";
    // ORDERING
    internal const string PageCart = "/cart";
}
