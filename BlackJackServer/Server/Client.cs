using System.Net;
using System.Net.Sockets;
using ServerLib;

namespace BJServer;

public class Client
{
    public readonly int Id;
    public string Email { get; set; } = string.Empty;
    public int RoomId { get; set; } = -1;
    
    public int DrawCard { get; set; } = 0;

    public TcpClient Socket { get; private set; }
    public NetworkStream Stream { get; private set; }
    
    private readonly CancellationTokenSource _cts;

    public Client(int id, TcpClient socket)
    {
        Id = id;

        _cts = new CancellationTokenSource();

        Socket = socket;
        Socket.ReceiveBufferSize = Constants.BufferSize;
        Socket.SendBufferSize = Constants.BufferSize;

        Stream = Socket.GetStream();
    }

    public void Connect()
    {
        var clientIpEp = Socket.Client.RemoteEndPoint as IPEndPoint;
        Console.WriteLine($"Client {Id} has connected! ({clientIpEp?.Address})");

        Receive();

        ServerEvent.Welcome(this);
    }

    private async void Receive()
    {
        while (_cts.IsCancellationRequested == false)
        {
            var bufferSize = Socket.ReceiveBufferSize;

            var buffer = new byte[bufferSize];
            var length = await Stream.ReadAsync(buffer, 0, bufferSize, _cts.Token);
            if (length <= 0)
            {
                Disconnect();
                return;
            }

            var data = EventData.Parse(buffer, length);
            Console.WriteLine($"Received event {data.Type} from client {Id}!");

            Server.HandleEvent(data, this);
        }
    }

    public async Task Send(EventData evt)
    {
        var buffer = EventData.Convert(evt);
        await Stream.WriteAsync(buffer, 0, buffer.Length, _cts.Token);
    }

    public void Initialize()
    {
        RoomId = -1;
        DrawCard = 0;
    }

    public void Disconnect()
    {
        _cts.Cancel();
        _cts.Dispose();

        Socket.Close();
        Stream.Close();

        Socket = null;
        Stream = null;

        Server.RejectClient(Id);

        Console.WriteLine($"Client {Id} has disconnected!");
    }
}