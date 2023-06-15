using System.Net;
using System.Net.Sockets;

namespace ServerLib;

public class Server
{
    public int Port { get; private set; }
    public int Capacity { get; private set; }

    private TcpListener _tcpListener;

    public static Dictionary<int, Client> Clients { get; private set; } = new();

    public Server(int port, int capacity)
    {
        Port = port;
        Capacity = capacity;
        
        _tcpListener = new TcpListener(IPAddress.Any, Port);
        
        Clients = new Dictionary<int, Client>(Capacity);
        for (var i = 1; i <= Capacity; i++)
        {
            Clients.Add(i, new Client());
        }
    }

    public async void Start(CancellationToken cancellationToken = default)
    {
        _tcpListener.Start();

        while (true)
        {
            try
            {
                var client = await _tcpListener.AcceptTcpClientAsync(cancellationToken);
                AddClient(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error accepting TCP client: " + ex.Message);
                throw;
            }
        }
    }

    private static void AddClient(TcpClient newClient)
    {
        for (var i = 1; i <= Clients.Count; i++)
        {
            var client = Clients[i];
            if (client.Socket == null)
            {
                client.Connect(i, newClient);
                break;
            }
        }
    }
}