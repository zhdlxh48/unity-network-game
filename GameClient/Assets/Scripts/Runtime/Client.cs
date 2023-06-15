using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace FiveBee.Runtime
{
    public class Client : MonoBehaviour
    {
        public const int DataBufferSize = 4096;
        
        public IPAddress Ip { get; private set; } = IPAddress.Loopback;
        public int Port { get; private set; } = 19948;
        
        private TcpClient _socket;
        private NetworkStream _stream;
        
        public int Id { get; private set; }
        
        private void Awake()
        {
            _socket = new TcpClient()
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };
        }

        private void Start()
        {
            Connect();
        }

        private async void Connect()
        {
            await _socket.ConnectAsync(Ip, Port);
            if (!_socket.Connected)
            {
                return;
            }
            
            _stream = _socket.GetStream();

            var msg = await Receive();
            Debug.Log(msg);
            ClientEvent.SendMessage(this, "Test");
        }

        public async void Send(Memory<byte> data, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_socket == null)
                {
                    return;
                }
                
                await _stream.WriteAsync(data, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private async Task<string> Receive(CancellationToken cancellationToken = default)
        {
            // while (!cancellationToken.IsCancellationRequested)
            // {
                var buf = new byte[DataBufferSize];
                var bufMem = buf.AsMemory(0, DataBufferSize);
                var result = await _stream.ReadAsync(bufMem, cancellationToken);
                // Send(bufMem, cancellationToken);
                return bufMem.ToMessage();
                // }
        }
    }
}
