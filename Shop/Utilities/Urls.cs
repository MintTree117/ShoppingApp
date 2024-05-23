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

    internal const string ApiLogin = IdentityBase + "/login";
    internal const string ApiTwoFactor = IdentityBase + "/2fa";
    internal const string ApiLoginRefresh = IdentityBase + "/refresh";

    internal const string ApiUpdatePassword = IdentityBase + "/password/manage";
    internal const string ApiForgotPassword = IdentityBase + "/password/forgot";
    internal const string ApiResetPassword = IdentityBase + "/password/reset";
    
    internal const string ParamReturnUrl = "ReturnUrl";

    internal const string PageHome = "";
    internal const string PageConfirmEmail = "";
    internal const string PageResendConfirm = "";
    internal const string PageLoginOrRegister = "";
    internal const string PageForgotPassword = "";
    internal const string PageLoginWithRecovery = "";
    internal const string PageAccountDeleted = "";
    internal const string PageAccountManage = "";
    
    internal const string PageCart = "";
}
