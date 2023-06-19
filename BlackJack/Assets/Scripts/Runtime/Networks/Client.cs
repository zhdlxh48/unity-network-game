using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ServerLib;
using UnityEngine.SceneManagement;
using EventType = ServerLib.EventType;

namespace Runtime.Networks
{
    public class Client : MonoBehaviour
    {
        private static Client _instance;
        public static Client Instance => _instance;
        
        private readonly string ip = IPAddress.Loopback.ToString();
        private readonly int port = 19948;

        [field: SerializeField] public int Id { get; set; } = -1;
        [field: SerializeField] public int RoomId { get; set; } = -1;
        
        [field: SerializeField] public int DrawCard { get; set; } = -1;

        private TcpClient _socket;
        private NetworkStream _stream;
    
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _socket = new TcpClient();
            _socket.ReceiveBufferSize = Constants.BufferSize;
            _socket.SendBufferSize = Constants.BufferSize;

            _cts = new CancellationTokenSource();
            
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }

        public async void Connect(string email)
        {
            Debug.Log("Client is starting...");
            await _socket.ConnectAsync(IPAddress.Parse(ip), port);
            Debug.Log("Client has connected!");

            _stream = _socket.GetStream();

            Receive();

            ClientEvent.Hello(this, email);
        }

        private async void Receive()
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

                var data = EventData.Parse(buffer, length);
                
                Debug.Log($"Received event {data.Type} from client!");
                Debug.Log("DEBUG JSON: " + Encoding.UTF8.GetString(buffer, 0, length));
            
                HandleEvent(data);
            }
        }

        public async Task Send(EventData data)
        {
            if (_stream == null)
            {
                return;
            }
            
            var buffer = EventData.Convert(data);
            await _stream.WriteAsync(buffer, 0, buffer.Length, _cts.Token);
        }

        public void HandleEvent(EventData data)
        {
            if (data.Type == EventType.Connect)
            {
                var msgData = data.GetData<MessageEventData>();
                var playerId = msgData.PlayerId;
                Debug.Log($"Client has connected! My id is {playerId}!");
                Id = playerId;
            }
            else if (data.Type == EventType.Room)
            {
                var roomData = data.GetData<RoomEventData>();
                if (roomData.Type == RoomEventData.EventType.Join)
                {
                    if (roomData.PlayerId == Id)
                    {
                        RoomId = roomData.RoomId;
                        Debug.Log($"Client has joined room! My room id is {RoomId}!");
                    }
                    else
                    {
                        Debug.Log($"Client {roomData.PlayerId} has joined room {RoomId}!");
                        // GameLogic.Instance.PlayerCards.Add(roomData.PlayerId, -1);
                    }
                }
                else if (roomData.Type == RoomEventData.EventType.Leave)
                {
                    if (roomData.PlayerId == -1)
                    {
                        Initialize();
                        GameLogic.Instance.PlayerCards.Clear();
                        Debug.Log($"Client has left room {RoomId}!");
                    }
                    else
                    {
                        Debug.Log($"Client {roomData.PlayerId} has left room {RoomId}!");
                        // GameLogic.Instance.PlayerCards.Remove(roomData.PlayerId);
                    }
                }
            }
            else if (data.Type == EventType.Game)
            {
                var gameData = data.GetData<GameEventData>();
                GameLogic.Instance.HandleLogic(gameData);
            }
        }

        private void Initialize()
        {
            RoomId = -1;
            DrawCard = -1;
        }

        private async void OnDestroy()
        {
            Debug.Log("Client is shutting down...");

            var data = new EventData(EventType.Disconnect, "Bye from the client!");
            await Send(data);

            Disconnect();
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
}