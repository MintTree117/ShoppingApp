namespace Shop.Utilities;

public static class Utils
{
    public static async Task Delay()
    {
        await Task.Delay( Consts.DelayTime );
    }
    
    public static string GetRatingCss( float rating, int level )
    {
        float compare = level;
        if (rating <= compare - 0.5f)
            return "fa-regular fa-star fa-xs rating-star";
        if (rating <= compare && rating > compare - 0.5f)
            return "fa fa-star-half-alt fa-xs rating-star";
        return "fa-solid fa-star fa-xs rating-star";
    }
}