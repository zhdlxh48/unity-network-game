using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Runtime;
using UnityEngine;

public class Client : MonoBehaviour
{
    private string ip = IPAddress.Loopback.ToString();
    private int port = 19948;
    
    private TcpClient _socket;
    private NetworkStream _stream;

    private CancellationTokenSource _cts;

    private void Awake()
    {
        _socket = new TcpClient();
        _socket.ReceiveBufferSize = Constants.BufferSize;
        _socket.SendBufferSize = Constants.BufferSize;
        
        _cts = new CancellationTokenSource();
    }

    private async void Start()
    {
        Debug.Log("Client is starting...");
        await _socket.ConnectAsync(IPAddress.Parse(ip), port);
        Debug.Log("Client has connected!");
        
        _stream = _socket.GetStream();

        Receive();
        
        var data = new ClientEvent(ClientEventType.Connect, "Hello from the client!");
        Send(data);
    }

    private async void OnDestroy()
    {
        Debug.Log("Client is shutting down...");
        
        var data = new ClientEvent(ClientEventType.Disconnect, "Bye from the client!");
        await Send(data);
        
        Disconnect();
    }

    private async Task Receive()
    {
        while (_cts.IsCancellationRequested == false) 
        {
            var bufferSize = _socket.ReceiveBufferSize;

            var buffer = new byte[bufferSize];
            var length = await _stream.ReadAsync(buffer, 0, bufferSize, _cts.Token);
            if (length <= 0)
            {
                Disconnect();
                return;
            }

            var data = ClientEvent.Parse(this, buffer, length);
            Debug.Log($"Received event {data.Type} from client!");
        }
    }
    
    public async Task Send(ClientEvent data)
    {
        var buffer = ClientEvent.Convert(data);
        await _stream.WriteAsync(buffer, 0, buffer.Length, _cts.Token);
    }

    public void Disconnect()
    {
        _cts.Cancel();
        _cts.Dispose();
        
        _socket.Close();
        _stream.Close();
        
        _socket = null;
        _stream = null;
    }
}
