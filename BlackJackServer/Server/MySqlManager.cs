using MySql.Data.MySqlClient;

namespace BJServer;

public class MySqlManager
{
    private string _server = "svc.sel4.cloudtype.app"; //DB 서버 주소, 로컬일 경우 localhost
    private int _port = 31261; //DB 서버 포트
    private string _database = "my_test_db"; //DB 이름
    private string _id = "zhdlxh48"; //계정 아이디
    private string _pw = "RealTjshd*499"; //계정 비밀번호
    private string _connectionAddress = "";

    public void Connect()
    {
        _connectionAddress = $"Server={_server}; Port={_port}; Database={_database}; Uid={_id}; Pwd={_pw};";
        Console.WriteLine(_connectionAddress);
    }

    public int GetScore(string email)
    {
        using (var mysql = new MySqlConnection(_connectionAddress))
        {
            var command = mysql.CreateCommand();
            command.CommandText = $"SELECT user_score FROM game_score WHERE user_email = '{email}';";
            
            mysql.Open();
            var reader = command.ExecuteReader();
            var score = 0;
            while (reader.Read())
            {
                score = reader.GetInt32(0);
            }
            mysql.Close();
            
            Console.WriteLine($"GetScore: {score}");

            return score;
        }
    }

    public bool HasScore(string email)
    {
        using (var mysql = new MySqlConnection(_connectionAddress))
        {
            var command = mysql.CreateCommand();
            command.CommandText = $"SELECT user_score FROM game_score WHERE user_email = '{email}';";
            
            mysql.Open();
            var reader = command.ExecuteReader();
            var available = reader.HasRows;
            mysql.Close();
            
            Console.WriteLine($"HasScore: {available}");

            return available;
        }
    }

    public void InsertScore(string email, int score)
    {
        using (var mysql = new MySqlConnection(_connectionAddress))
        {
            var command = mysql.CreateCommand();
            command.CommandText = $"INSERT INTO game_score (user_email, user_score) VALUES ('{email}', '{score}');";
            
            mysql.Open();
            if (command.ExecuteNonQuery() != 1)
            {
                Console.WriteLine("Insert Error");
            }
            mysql.Close();
        }
    }

    public void UpdateScore(string email, int score)
    {
        using (var mysql = new MySqlConnection(_connectionAddress))
        {
            var command = mysql.CreateCommand();
            command.CommandText = $"UPDATE game_score SET user_score = user_score + {score} where user_email = '{email}';";
            
            mysql.Open();
            command.ExecuteNonQuery();
            mysql.Close();
        }
    }
}