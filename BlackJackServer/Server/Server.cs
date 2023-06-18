using System.Net;
using System.Net.Sockets;

namespace Server;

public class Server
{
    private static IPAddress _ip;
    private static int _port;
    
    private static TcpListener _listener;
    
    public static Dictionary<int, Client?> Clients { get; private set; } = new();
    public static Dictionary<int, Room?> Rooms { get; private set; } = new();

    public static void Start(IPAddress ip, int port)
    {
        _ip = ip;
        _port = port;

        for (var i = 0; i < 100; i++)
        {
            Clients.Add(i, null);
        }
        
        _listener = new TcpListener(_ip, _port);
        _listener.Start();
        
        AsyncAccept();
    }

    private static async void AsyncAccept()
    {
        while (true)
        {
            Console.WriteLine("Waiting for client...");
            var client = await _listener.AcceptTcpClientAsync();
            AssignClient(client);
        }
    }

    private static void AssignClient(TcpClient client)
    {
        for (int i = 0; i < Constants.MaxClients; i++)
        {
            if (Clients[i] != null)
            {
                continue;
            }
            
            Console.WriteLine($"Accepted client! Assigning id {i}...");

            Clients[i] = new Client(i, client);
            Clients[i]?.Connect();
            return;
        }
        
        Console.WriteLine("Server is full!");
    }
    
    public static void RejectClient(int id)
    {
        Clients[id] = null;
    }

    public static bool AssignRoom(Client client)
    {
        for (int i = 0; i < Constants.MaxRooms; i++)
        {
            var room = Rooms[i];
            
            if (room == null)
            {
                room = new Room(Constants.MaxRoomClients);
            }
            else if (room.IsFull)
            {
                continue;
            }
            
            room.AddClient(client);
            client.RoomId = i;
            return true;
        }
        
        Console.WriteLine("No rooms available!");
        return false;
    }
    
    public static void RejectRoom(Client client)
    {
        var room = Rooms[client.RoomId];
        if (room == null)
        {
            Console.WriteLine($"Client {client.Id} tried to reject a room, but they aren't in one!");
        }
        else
        {
            room.RemoveClient(client);
        }
        
        client.RoomId = -1;
    }
}