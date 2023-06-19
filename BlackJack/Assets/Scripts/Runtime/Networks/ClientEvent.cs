using ServerLib;

namespace Runtime.Networks
{
    public static class ClientEvent
    {
        public static async void Hello(Client client, string email)
        {
            var msg = new MessageEventData(client.Id, email);
            var data = new EventData(EventType.Connect, msg);
            await client.Send(data);
        }
        
        public static async void EnterRoom(Client client)
        {
            var roomData = new RoomEventData(RoomEventData.EventType.Join, -1);
            var data = new EventData(EventType.Room, roomData);
            await client.Send(data);
        }
        
        public static async void LeaveRoom(Client client)
        {
            var roomData = new RoomEventData(RoomEventData.EventType.Leave, -1);
            var data = new EventData(EventType.Room, roomData);
            await client.Send(data);
        }
    }
}