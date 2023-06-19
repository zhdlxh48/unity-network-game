using ServerLib;

namespace Runtime.Networks
{
    public static class GameEvent
    {
        public static async void DrawCard(Client client)
        {
            if (GameLogic.Instance.CurrentEventType != GameEventData.EventType.CardDrawStart)
            {
                return;
            }
            
            GameLogic.Instance.CurrentEventType = GameEventData.EventType.CardDrawEnd;
            
            var roomData = new GameEventData(GameEventData.EventType.CardDrawEnd, client.RoomId, client.Id);
            var data = new EventData(EventType.Game, roomData);
            await client.Send(data);
        }
    }
}