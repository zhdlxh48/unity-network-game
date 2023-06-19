using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace ServerLib;

public class EventData
{
    [JsonConverter(typeof(StringEnumConverter))]
    public EventType Type;
    
    public object Data;

    public EventData(EventType type, object data)
    {
        Type = type;
        Data = data;
    }

    public static EventData Parse(byte[] buffer, int length)
    {
        var json = Encoding.UTF8.GetString(buffer, 0, length);
        return JsonConvert.DeserializeObject<EventData>(json);
    }

    public static byte[] Convert(EventType type, object data)
    {
        var evtData = new EventData(type, data);
        return Convert(evtData);
    }

    public static byte[] Convert(EventData data)
    {
        var json = JsonConvert.SerializeObject(data);
        return Encoding.UTF8.GetBytes(json);
    }

    public T GetData<T>()
    {
        var jObj = (JObject)Data;
        Console.WriteLine(jObj.ToString());
        return jObj.ToObject<T>();
    }
}

public enum EventType
{
    None,
    Connect,
    Disconnect,
    Room,
    Chat,
    Game
}