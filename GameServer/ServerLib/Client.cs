using System.Net.Sockets;

namespace ServerLib;

public class Client
{
    public const int DataBufferSize = 4096;
    
    public int Id { get; private set; }
    
    public TcpClient? Socket { get; private set; }
    private NetworkStream _stream;

    public Client()
    {
        Id = 0;
        Socket = null;
    }
    
    public void Connect(int id, TcpClient clientSocket)
    {
        Id = id;
        
        Socket = clientSocket;
        Socket.ReceiveBufferSize = DataBufferSize;
        Socket.SendBufferSize = DataBufferSize;
        
        _stream = Socket.GetStream();
        
        ServerEvent.SendEvent(Id, ServerEvent.EventType.Welcome);

        Receive();
    }

    private async void Receive(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var buf = new byte[DataBufferSize];
            var bufMem = buf.AsMemory(0, DataBufferSize);
            var result = await _stream.ReadAsync(bufMem, cancellationToken);
            Console.WriteLine(result);
            Send(bufMem, cancellationToken);
        }
    }

    public async void Send(Memory<byte> data, CancellationToken cancellationToken = default)
    {
        if (Socket == null)
        {
            return;
        }

        await _stream.WriteAsync(data, cancellationToken);
    }
}