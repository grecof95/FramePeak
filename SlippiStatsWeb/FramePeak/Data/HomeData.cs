using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace FramePeak.Data
{
    public class HomeData
    {
        public readonly string connString;

        public HomeData(string connString)
        {
            this.connString = connString;
        }

        public bool ValidateUser(string username, string password)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public string CreateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return "empty";
            }

            using var conn = new SqliteConnection(connString);
            conn.Open();

            // Check if username already exists
            using (var checkCmd = conn.CreateCommand())
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                checkCmd.Parameters.AddWithValue("@username", username);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                if (exists)
                {
                    return "exists";
                }
            }

            // Insert new user
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            int result = cmd.ExecuteNonQuery();
            return result > 0 ? "success" : "fail";
        }

        public int GetUserId(string username, string password)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT UserID FROM Users WHERE Username = @username AND Password = @password";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : -1;
        }

        public int GetMatchCountForUser(int userId)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM SlippiSubmissions WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        public string GetMostPlayedCharacter(int userId)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CharacterPlayed, COUNT(*) AS PlayCount FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserId = @userId GROUP BY CharacterPlayed ORDER BY PlayCount DESC LIMIT 1;";
            cmd.Parameters.AddWithValue("@userId", userId);

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public string GetMostPlayedOpponent(int userId)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CharacterAgainst, COUNT(*) AS PlayCount FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId GROUP BY CharacterAgainst ORDER BY PlayCount DESC LIMIT 1;";
            cmd.Parameters.AddWithValue("@userId", userId);

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public List<string> GetUserTop3CharacterWinRate(int userId)
        {
            var topCharacters = new List<string>();
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CharacterPlayed, ROUND(CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / COUNT(*), 2) || '%' AS WinRate FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId GROUP BY CharacterPlayed ORDER BY CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / COUNT(*) DESC LIMIT 3;";
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string character = reader.GetString(0);
                string winRate = reader.GetString(1);
                topCharacters.Add($"{character} - {winRate}");
            }
            return topCharacters;
        }

        public List<string> GetUserTop3CharacterUsage(int userId)
        {
            var topUsage = new List<string>();
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CharacterPlayed, COUNT(*) AS PlayCount FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId GROUP BY CharacterPlayed ORDER BY PlayCount DESC LIMIT 3;";
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string character = reader.GetString(0);
                String characterCount = reader.GetString(1);
                topUsage.Add($"{character} - {characterCount}");
            }

            return topUsage;
        }

        public List<string> GetUserTop3Maps(int userId)
        {
            var topMapRate = new List<string>();
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT MapPlayed, ROUND(CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / COUNT(*), 2) || '%' AS WinRate FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId GROUP BY MapPlayed ORDER BY CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / COUNT(*) DESC LIMIT 3;";
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string map = reader.GetString(0);
                string winRate = reader.GetString(1);
                topMapRate.Add($"{map} - {winRate}");
            }

            return topMapRate;
        }

        public string GetUserOverallLCancelPercent(int userId)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ROUND(AVG(LCancelPercent), 2) || '%' AS AverageLCancelPercent FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId;";
            cmd.Parameters.AddWithValue("@userId", userId);

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public List<string> GetCharactersPlayedByUser(int userId)
        {
            var characters = new List<string>();

            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT DISTINCT CharacterPlayed 
                                FROM SlippiGameData 
                                JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID 
                                WHERE SlippiSubmissions.UserID = @userId;";
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                characters.Add(reader.GetString(0));
            }

            return characters;
        }

        public string GetCharacterWins(int userId, string character)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(WinLoss) FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId and SlippiGameData.CharacterPlayed = @character and WinLoss = 'Win';";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@character", character);

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public string GetCharacterLosses(int userId, string character)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(WinLoss) FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId and SlippiGameData.CharacterPlayed = @character and WinLoss = 'Loss';";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@character", character);

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public string GetCharacterAvgStocksTaken(int userId, string character)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ROUND(AVG(StocksTaken), 2) AS AverageStocksTaken FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId and SlippiGameData.CharacterPlayed = @character;\r\n";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@character", character);

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public string GetCharacterAvgStocksLost(int userId, string character)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ROUND(AVG(StocksLost), 2) AS AverageStocksLost FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId and SlippiGameData.CharacterPlayed = @character;\r\n";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@character", character);

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public string GetCharacterAvgDamageDealt(int userId, string character)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ROUND(AVG(DamageDone), 2) || '%' AS AverageDamageDone FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId and SlippiGameData.CharacterPlayed = @character;\r\n";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@character", character);

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public string GetCharacterAvgDamageTaken(int userId, string character)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ROUND(AVG(DamageTaken), 2) || '%' AS AverageDamageTaken FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId and SlippiGameData.CharacterPlayed = @character;\r\n";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@character", character);

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public string GetCharacterAvgLCancel(int userId, string character)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ROUND(AVG(LCancelPercent), 2) || '%' AS AvgCharLCancelPercent FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID WHERE SlippiSubmissions.UserID = @userId and SlippiGameData.CharacterPlayed = @character;";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@character", character);

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public string GetTopWinRateCharacter(int userId)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                                SELECT CharacterPlayed 
                                FROM SlippiGameData 
                                JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID 
                                WHERE SlippiSubmissions.UserID = @userId 
                                GROUP BY CharacterPlayed 
                                ORDER BY 
                                CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 1.0 / COUNT(*) DESC 
                                LIMIT 1;";
            cmd.Parameters.AddWithValue("@userId", userId);

            return Convert.ToString(cmd.ExecuteScalar());
        }
    }
}