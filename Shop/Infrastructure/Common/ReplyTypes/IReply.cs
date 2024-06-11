namespace Shop.Infrastructure.Common.ReplyTypes;

public interface IReply
{
    public string Message();
    public bool IsOkay { get; init; }

    public static Reply<bool> True() => Reply<bool>.True( true );
    public static Reply<bool> False( string msg ) => Reply<bool>.False( msg );
    public static Reply<bool> False( IReply reply, string msg ) => Reply<bool>.False( msg );
    public static Reply<bool> False( IReply reply ) => Reply<bool>.False( reply );
}