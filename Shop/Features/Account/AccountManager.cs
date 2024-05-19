using Shop.Features.Account.Types;
using Shop.Infrastructure.Http;
using ShopApplication.Common.Optionals;
using ShopWeb.Utilities;

namespace Shop.Features.Account;

public class AccountManager( IHttpService httpService )
{
    readonly IHttpService http = httpService;
    Opt<AccountModelNew> cachedModel = Opt<AccountModelNew>.None( "Nothing here." );

    internal async Task<Opt<AccountModelNew>> FetchAccountDetails()
    {
        if (cachedModel.IsOkay())
            return cachedModel;

        cachedModel = await http.TryGetRequest<AccountModelNew>( Urls.ApiGetAccountDetails );
        return cachedModel;
    }
}