namespace Server;

public class Room
{
    private int _maxCount;
    
    public bool IsFull => clients.Count >= _maxCount;

    private List<Client> clients = new();

    public Room(int max)
    {
        _maxCount = max;
    }
    
    public void AddClient(Client client)
    {
        clients.Add(client);
    }
    
    public void RemoveClient(Client client)
    {
        clients.Remove(client);
    }
}