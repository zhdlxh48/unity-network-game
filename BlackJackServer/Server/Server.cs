using System.Net;
using System.Net.Sockets;
using ServerLib;

namespace BJServer;

public class Server
{
    private static IPAddress _ip;
    private static int _port;

    private static TcpListener _listener;

    public static Dictionary<int, Client> Clients { get; } = new();
    public static Dictionary<int, Room> Rooms { get; } = new();

    public static void Start(IPAddress ip, int port)
    {
        _ip = ip;
        _port = port;

        for (var i = 0; i < Constants.MaxClients; i++)
        {
            Clients.Add(i, null);
        }

        for (var i = 0; i < Constants.MaxRooms; i++)
        {
            Rooms.Add(i, null);
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

    public static void HandleEvent(EventData data, Client client)
    {
        // Server, Client
        if (data.Type == EventType.Connect)
        {
            var email = data.GetData<MessageEventData>();
            client.Email = email.Message;
            
            Console.WriteLine($"Client {client.Id} has connected! ({client.Email})");
        }
        else if (data.Type == EventType.Disconnect)
        {
            client.Disconnect();

            if (TryLeaveRoom(client, out var roomId))
            {
                Console.WriteLine($"Client {client.Id} successfully left room {roomId}!");
            }
        }
        // Room
        else if (data.Type == EventType.Room)
        {
            var roomData = data.GetData<RoomEventData>();
            // Join
            if (roomData.Type == RoomEventData.EventType.Join)
            {
                if (TryEnterRoom(client, out var roomId))
                {
                    ServerEvent.EnterRoom(client, roomId, client.Id);
                    Console.WriteLine($"Client {client.Id} joined room {roomId}!");

                    foreach (var roomClient in Rooms[roomId].Clients.Where(roomClient => roomClient != client))
                    {
                        ServerEvent.EnterRoom(roomClient, roomId, client.Id);
                    }
                    
                    // Room Full -> Game Start
                    if (Rooms[roomId].IsFull)
                    {
                        Console.WriteLine($"Room {roomId} is full! Starting game...");
                        GameEvent.StartGame(Rooms[roomId]);
                    }
                }
                else
                {
                    Console.WriteLine($"Client {client.Id} failed to join room {roomId}!");
                }
            }
            // Leave
            else if (roomData.Type == RoomEventData.EventType.Leave)
            {
                if (TryLeaveRoom(client, out var roomId))
                {
                    ServerEvent.LeaveRoom(client, roomId, client.Id);
                    Console.WriteLine($"Client {client.Id} left room {roomId}!");
                    
                    foreach (var roomClient in Rooms[roomId].Clients.Where(roomClient => roomClient != client))
                    {
                        ServerEvent.LeaveRoom(roomClient, roomId, client.Id);
                    }
                }
                else
                {
                    Console.WriteLine($"Client {client.Id} failed to leave room {roomId}!");
                }
            }
        }
        // Game
        else if (data.Type == EventType.Game)
        {
            var gameData = data.GetData<GameEventData>();
            if (gameData.Type == GameEventData.EventType.CardDrawEnd)
            {
                var roomId = gameData.RoomId;
                var room = Rooms[roomId];
                var clients = room.Clients;
                
                Console.WriteLine("Current cards:");
                foreach (var checkClient in clients)
                {
                    Console.WriteLine($"Player {checkClient.Id}: {checkClient.DrawCard}");
                }
                Console.WriteLine("---");

                foreach (var drawClient in clients)
                {
                    var card = drawClient.DrawCard;
                    if (card > 0)
                    {
                        continue;
                    }
                    
                    Console.WriteLine($"Now, Player {drawClient.Id} have to draw a card!");
                    
                    GameEvent.DrawCard(room, drawClient);
                    return;
                }
                
                Console.WriteLine($"All clients in room {roomId} drew their cards!");
                
                GameEvent.GameResult(room);
            }
        }
    }

    #region Client Events

    private static void AssignClient(TcpClient client)
    {
        for (var i = 0; i < Constants.MaxClients; i++)
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

    #endregion

    #region Room Events

    public static bool TryEnterRoom(Client client, out int roomId)
    {
        if (client.RoomId >= 0)
        {
            Console.WriteLine($"Client {client.Id} is already in a room!");
            
            roomId = -1;
            return false;
        }
        
        for (var i = 0; i < Constants.MaxRooms; i++)
        {
            if (Rooms[i] == null)
            {
                Rooms[i] = new Room(i, Constants.MaxRoomClients);
            }

            if (Rooms[i].IsFull)
            {
                continue;
            }

            Rooms[i].AddClient(client);
            client.RoomId = i;
            roomId = i;
            return true;
        }

        Console.WriteLine("No rooms available!");
        
        roomId = -1;
        return false;
    }

    public static bool TryLeaveRoom(Client client, out int roomId)
    {
        var room = Rooms[client.RoomId];
        if (room == null)
        {
            Console.WriteLine($"Client {client.Id} tried to reject a room, but they aren't in one!");
            
            roomId = -1;
            return false;
        }
        
        room.RemoveClient(client);
        client.Initialize();
        
        roomId = room.Id;
        return true;
    }

    #endregion
}