using System;

namespace FiveBee.Runtime
{
    public static class ClientEvent
    {
        public static void SendMessage(Client client, string message)
        {
            client.Send(message.ToMemory());
        }
    
        public enum EventType
        {
            Welcome,
            Message,
            Disconnect
        }
    }
}