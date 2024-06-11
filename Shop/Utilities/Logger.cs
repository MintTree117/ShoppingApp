using Shop.Infrastructure.Common.ReplyTypes;

namespace Shop.Utilities;

public static class Logger
{
    public const string Divider = "---------------------------------------------------------------------------------------------------------------------------------------";

    public static void Log( string m )
    {
        Console.WriteLine( m );
        Console.WriteLine( Divider );
    }
    public static void Log( string m1, string m2 )
    {
        Console.WriteLine( m1 );
        Console.WriteLine( m2 );
        Console.WriteLine( Divider );
    }
    public static void LogError( string m )
    {
        Console.Error.WriteLine( m );
        Console.Error.WriteLine(Divider);
    }
    public static void LogError( string m1, string m2 )
    {
        Console.Error.WriteLine( m1 );
        Console.Error.WriteLine( m2 );
        Console.Error.WriteLine( Divider );
    }
    public static void LogError( Exception e, string m )
    {
        Console.Error.WriteLine( m );
        Console.Error.WriteLine( Divider );
        Console.Error.WriteLine( e );
    }
    public static void LogError( Exception e, string m1, string m2 )
    {
        Console.Error.WriteLine( m1 );
        Console.Error.WriteLine( m2 );
        Console.Error.WriteLine( Divider );
        Console.Error.WriteLine( e );
    }
    public static void LogError( IReply reply, string m )
    {
        Console.WriteLine( $"{m}{reply.Message()}" );
        Console.WriteLine( Divider );
    }
}