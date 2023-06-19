using ServerLib;

namespace BJServer;

public static class ServerEvent
{
    public static async void Welcome(Client client)
    {
        var msg = new MessageEventData(client.Id, "Welcome to the server!");
        var data = new EventData(EventType.Connect, msg);
        await client.Send(data);
    }

    public static async void EnterRoom(Client client, int roomId, int playerId = 0)
    {
        var roomData = new RoomEventData(RoomEventData.EventType.Join, roomId, roomId >= 0);
        roomData.PlayerId = client.Id;
        var data = new EventData(EventType.Room, roomData);
        await client.Send(data);
    }

    public static async void LeaveRoom(Client client, int roomId, int playerId = 0)
    {
        var roomData = new RoomEventData(RoomEventData.EventType.Leave, roomId, roomId >= 0);
        roomData.PlayerId = client.Id;
        var data = new EventData(EventType.Room, roomData);
        await client.Send(data);
    }
}