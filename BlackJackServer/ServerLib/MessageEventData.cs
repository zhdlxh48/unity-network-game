namespace ServerLib;

public class MessageEventData : BaseEventData
{
    public int PlayerId;
    
    public string Message;

    public MessageEventData(int playerId, string message, bool success = true) : base(success)
    {
        PlayerId = playerId;
        Message = message;
    }
}