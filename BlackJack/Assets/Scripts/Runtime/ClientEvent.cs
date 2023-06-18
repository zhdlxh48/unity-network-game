using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Runtime
{
    public class ClientEvent
    {
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public ClientEventType Type;

        public object Data;
    
        public ClientEvent(ClientEventType type, object data)
        {
            Type = type;
            Data = data;
        }
    
        public static ClientEvent Parse(Client client, byte[] buffer, int length)
        {
            var json = Encoding.UTF8.GetString(buffer, 0, length);
            Debug.Log(json);

            var eventObj = JsonConvert.DeserializeObject<ClientEvent>(json);
            return eventObj;
        }
    
        public static byte[] Convert(ClientEvent data)
        {
            var json = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
    
    public enum ClientEventType
    {
        Connect,
        Disconnect,
        Message,
        CreateRoom,
        JoinRoom,
        LeaveRoom,
        DestroyRoom,
        InGame
    }
}