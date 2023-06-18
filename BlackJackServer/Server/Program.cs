using System.Net;

namespace Server;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Starting server...");

        var thread = new Thread(ServerThread);
        // thread.IsBackground = true;
        thread.Start();

        Console.Read();
    }

    public static void ServerThread()
    {
        Server.Start(IPAddress.Any, 19948);
        
        Console.WriteLine("Server started!");
    }
}