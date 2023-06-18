using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

public class Client
{
    public readonly int Id;
    public int RoomId { get; set; } = -1;
    
    public TcpClient Socket { get; private set; }
    public NetworkStream Stream { get; private set; }

    private CancellationTokenSource _cts;

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
        
        var data = new ClientEvent(ClientEventType.Connect, "Hello from the server!");
        Send(data);
    }

    private async Task Receive()
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

            var data = ClientEvent.Parse(this, buffer, length);
            Console.WriteLine($"Received event {data.Type} from client {Id}!");
        }
    }
    
    public async Task Send(ClientEvent data)
    {
        var buffer = ClientEvent.Convert(data);
        await Stream.WriteAsync(buffer, 0, buffer.Length, _cts.Token);
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