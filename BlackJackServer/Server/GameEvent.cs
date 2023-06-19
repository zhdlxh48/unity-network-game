using ServerLib;

namespace BJServer;

public static class GameEvent
{
    public static async void StartGame(Room room)
    {
        foreach (var roomClient in room.Clients)
        {
            var gameData = new GameEventData(GameEventData.EventType.Start, room.Id, roomClient.Id);
            var data = new EventData(EventType.Game, gameData);
            await roomClient.Send(data);
        }

        await Task.Delay(TimeSpan.FromSeconds(2f));

        // Draw Start Client
        var drawClient = room.Clients[0];
        DrawCard(room, drawClient);
    }

    public static async void DrawCard(Room room, Client drawClient)
    {
        var clients = room.Clients;
        
        var rand = new Random();
        var randNum = rand.Next(1, 13);
        drawClient.DrawCard = randNum;
        
        Console.WriteLine($"Client {drawClient.Id} Will Draw Card: {randNum}");

        foreach (var client in clients)
        {
            var gameDrawData =
                new GameEventData(
                    client == drawClient ? GameEventData.EventType.CardDrawStart : GameEventData.EventType.CardDrawWait,
                    room.Id, drawClient.Id);
            gameDrawData.DrawCard = randNum;
            var drawData = new EventData(EventType.Game, gameDrawData);
            
            await client.Send(drawData);
        }
    }

    public static async void GameResult(Room room)
    {
        var clients = room.Clients;
        var winner = room.Clients[0];
        foreach (var client in clients)
        {
            winner = client.DrawCard > winner.DrawCard ? client : winner;
        }
        
        var gameWinnerData = new GameEventData(GameEventData.EventType.Win, room.Id, winner.Id);
        var winnerData = new EventData(EventType.Game, gameWinnerData);
        
        await winner.Send(winnerData);

        var loserHighScore = 0;
        foreach (var client in clients.Where(client => winner != client))
        {
            loserHighScore = client.DrawCard > loserHighScore ? client.DrawCard : loserHighScore;
            var gameLoserData = new GameEventData(GameEventData.EventType.Lose, room.Id, client.Id);
            var loserData = new EventData(EventType.Game, gameLoserData);
            
            await client.Send(loserData);
        }
        
        // DB 연결

        var points = winner.DrawCard - loserHighScore;
        Console.WriteLine($"Winner gets {points} points!");

        var db = new MySqlManager();
        db.Connect();

        if (db.HasScore(winner.Email))
        {
            db.UpdateScore(winner.Email, points);
        }
        else
        {
            db.InsertScore(winner.Email, points);
        }
    }
}