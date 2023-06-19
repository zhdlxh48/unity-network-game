using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib;

public class RoomEventData : BaseEventData
{
    public enum EventType
    {
        Create,
        Join,
        Leave,
        Destroy
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public EventType Type;

    public int RoomId;
    
    public int PlayerId;

    public RoomEventData(EventType type, int id, bool success = true) : base(success)
    {
        Type = type;
        RoomId = id;
    }
}