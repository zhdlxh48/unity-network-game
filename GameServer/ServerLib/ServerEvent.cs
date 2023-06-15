namespace ServerLib;

public static class ServerEvent
{
    private const string WelcomeMessage = "Welcome to the server!";
    
    public static void SendMessage(Client client, string message)
    {
        client.Send(message.ToMemory());
    }
    
    public static void SendMessage(int id, string message)
    {
        SendMessage(GetClient(id), message);
    }
    
    public static void SendEvent(Client client, EventType type)
    {
        var message = "";
        switch (type)
        {
            case EventType.Welcome:
                message = WelcomeMessage;
                break;
            case EventType.Message:
                break;
            case EventType.Disconnect:
                break;
        }
        
        SendMessage(client, message);
    }

    public static void SendEvent(int id, EventType type)
    {
        SendEvent(GetClient(id), type);
    }
    
    private static Client GetClient(int id)
    {
        return Server.Clients[id];
    }
    
    public enum EventType
    {
        Welcome,
        Message,
        Disconnect
    }
}