using Shop.Infrastructure.Common.Optionals;
using Shop.Infrastructure.Http;

namespace Shop.Infrastructure.Catalog;

public sealed class CategoriesService( HttpService httpService )
{
    readonly HttpService http = httpService;
    Opt<CategoryData> categories = Opt<CategoryData>.None();

    public async Task<Opt<CategoryData>> GetCategories()
    {
        if (categories.IsOkay)
            return categories;

        Opt<List<Category>> fetchResult = await http.TryGetRequest<List<Category>>( "Get Categories" );

        if (!fetchResult.IsOkay)
            return Opt<CategoryData>.None( fetchResult );

        categories = Opt<CategoryData>.With( CategoryData.Create( fetchResult.Data ) );
        return categories;
    }
}