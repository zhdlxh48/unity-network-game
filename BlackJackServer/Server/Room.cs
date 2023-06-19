namespace BJServer;

public class Room
{
    private readonly int _maxCount;

    public readonly List<Client> Clients = new();
    public readonly int Id;

    public Room(int id, int max)
    {
        Id = id;
        _maxCount = max;
    }

    public bool IsFull => Clients.Count >= _maxCount;

    public void AddClient(Client client)
    {
        Clients.Add(client);
    }

    public void RemoveClient(Client client)
    {
        Clients.Remove(client);
    }
}