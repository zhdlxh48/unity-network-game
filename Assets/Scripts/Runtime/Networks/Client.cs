using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace FiveBee.Runtime.Networks
{
    public class Client : MonoBehaviour
    {
        private static Client _instance;
        public static Client Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Client>();
                }
                if (_instance == null)
                {
                    Debug.LogWarning($"Instance of {nameof(Client)} not found, creating new one...");
                    _instance = new GameObject("Client").AddComponent<Client>();
                    
                    Initialize();
                }
                
                return _instance;
            }
        }
        
        public const int DataBufferSize = 4096;

        [field: SerializeField] public string Ip { get; private set; } = IPAddress.Loopback.ToString();
        [field: SerializeField] public int Port { get; private set; } = 19948;
        [field: SerializeField] public int MyId { get; private set; } = 0;
        public TcpData Tcp { get; private set; }

        // private void Awake()
        // {
        //     if (_instance == null)
        //     {
        //         _instance = this;
        //     }
        //     else if (_instance != this)
        //     {
        //         Debug.LogWarning($"Instance of {nameof(Client)} already exists, destroying {gameObject.name}...");
        //         Destroy(gameObject);
        //     }
        // }
   
        private void Start()
        {
            Initialize();
        }

        private static void Initialize()
        {
            Instance.Tcp = new TcpData();
        }

        public void ConnectToServer()
        {
            Tcp.Connect();
        }
    }

    public class TcpData
    {
        private TcpClient _socket;

        private NetworkStream _stream;
        private byte[] _receiveBuffer;
        
        public void Connect()
        {
            var bufferSize = Client.DataBufferSize;
            
            _socket = new TcpClient
            {
                ReceiveBufferSize = bufferSize,
                SendBufferSize = bufferSize
            };

            _receiveBuffer = new byte[bufferSize];
            _socket.BeginConnect(Client.Instance.Ip, Client.Instance.Port, ConnectCallback, _socket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            _socket.EndConnect(ar);

            if (!_socket.Connected)
            {
                return;
            }
            
            Debug.Log($"Connected to server at {Client.Instance.Ip}:{Client.Instance.Port}.");
            
            _stream = _socket.GetStream();

            _stream.BeginRead(_receiveBuffer, 0, Client.DataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var length = _stream.EndRead(ar);
                if (length <= 0)
                {
                    // TODO: disconnect
                    return;
                }
                
                var data = new byte[length];
                Array.Copy(_receiveBuffer, data, length);
            }
            catch (Exception ex)
            {
                // TODO: disconnect
                
                Debug.LogError($"Error receiving TCP data: {ex}");
                
                throw;
            }
        }
    }
}
