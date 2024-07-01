using Shop.Types.Common.ReplyTypes;

namespace Shop.Utilities;

public static class Logger
{
    public const string Divider = "---------------------------------------------------------------------------------------------------------------------------------------";

    public static void Log( string m )
    {
        Console.WriteLine( m + Divider );
    }
    public static void Log( string m1, string m2 )
    {
        Console.WriteLine( m1 + Divider );
        Console.WriteLine( m2 + Divider );
    }
    public static void LogError( string m )
    {
        Console.Error.WriteLine( m + Divider );
    }
    public static void LogError( string m1, string m2 )
    {
        Console.Error.WriteLine( m1 + Divider );
        Console.Error.WriteLine( m2 + Divider );
    }
    public static void LogError( Exception e, string m )
    {
        Console.Error.WriteLine( m + Divider );
        Console.Error.WriteLine( e + Divider );
    }
    public static void LogError( Exception e, string m1, string m2 )
    {
        Console.Error.WriteLine( m1 + Divider );
        Console.Error.WriteLine( m2 + Divider );
        Console.Error.WriteLine( e + Divider );
    }
    public static void LogError( IReply reply, string m )
    {
        Console.WriteLine( $"{m}{reply.GetMessage()}" + Divider );
    }
}