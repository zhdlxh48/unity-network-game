using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib;

public class GameEventData : BaseEventData
{
    public enum EventType
    {
        Start, End, CardDrawWait, CardDrawStart, CardDrawEnd, Raise, Call, Win, Lose, Draw
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public EventType Type;
    
    public int RoomId;

    public int PlayerId;
    
    public int DrawCard;
    public int RaiseAmount;
    
    public GameEventData(EventType type, int roomId, int playerId, bool success = true) : base(success)
    {
        Type = type;
        
        RoomId = roomId;
        PlayerId = playerId;
    }
}