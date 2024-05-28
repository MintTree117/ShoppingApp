namespace Shop.Utilities;

internal static class Urls
{
    internal const string ApiBase = "https://localhost:7212/api";
    internal const string IdentityBase = ApiBase + "/identity";
    
    internal const string ApiGetAccountDetails = IdentityBase + "/account/view";
    internal const string ApiUpdateAccountDetails = IdentityBase + "/account/update";
    internal const string ApiDeleteAccount = IdentityBase + "/account/delete";

    internal const string ApiGetAddresses = IdentityBase + "/address/view";
    internal const string ApiUpdateAddress = IdentityBase + "/address/update";
    internal const string ApiDeleteAddress = IdentityBase + "/address/delete";

    internal const string ApiRegister = IdentityBase + "/register";
    internal const string ApiConfirmEmail = IdentityBase + "/email/confirm";
    internal const string ApiResendConfirm = IdentityBase + "/email/resend";
    
    internal const string ApiLogout = IdentityBase + "/logout";
    internal const string ApiLogin = IdentityBase + "/login";
    internal const string ApiTwoFactor = IdentityBase + "/2fa";
    internal const string ApiLoginRefresh = IdentityBase + "/refresh";

    internal const string ApiUpdatePassword = IdentityBase + "/password/manage";
    internal const string ApiForgotPassword = IdentityBase + "/password/forgot";
    internal const string ApiResetPassword = IdentityBase + "/password/reset";
    
    internal const string ParamReturnUrl = "ReturnUrl";

    internal const string PageHome = "/home";
    internal const string PageConfirmEmail = "/confirm-email";
    internal const string PageResendConfirm = "/resend-confirmation-email";
    internal const string PageLoginOrRegister = "/login-or-register";
    internal const string PageLogout = "/logout";
    internal const string PageForgotPassword = "/forgot-password";
    internal const string PageLoginWithRecovery = "/recovery-login";
    internal const string PageAccountDeleted = "/account-deleted";
    internal const string PageAccountManage = "/manage-account";
    internal const string PageOrdersManage = "/manage-orders";
    
    internal const string PageCart = "/cart";
}
