using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace FramePeak.Data
{
    public class GlobalData
    {
        public readonly string connString;

        public GlobalData(string connString)
        {
            this.connString = connString;
        }

        public string GetGlobalOverallLCancelPercent()
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ROUND(AVG(LCancelPercent), 2) || '%' AS AverageLCancelPercent FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID";

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public string GetGlobalDamageDealt()
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ROUND(AVG(DamageDone), 2) || '%' AS AverageDamageDone FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID;";

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public string GetGlobalDamageTaken()
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ROUND(AVG(DamageTaken), 2) || '%' AS AverageDamageTaken FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID;";

            return Convert.ToString(cmd.ExecuteScalar());
        }

        public int GetGlobalMatchCount()
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM SlippiSubmissions";

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List <string> GetGlobalTop3CharacterWinRate()
        {
            var topCharacters = new List<string>();
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CharacterPlayed, ROUND(CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / COUNT(*), 2) || '%' AS WinRate FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID GROUP BY CharacterPlayed HAVING COUNT(*) >= 3 ORDER BY CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / COUNT(*) DESC LIMIT 3;\r\n";


            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string character = reader.GetString(0);
                string winRate = reader.GetString(1);
                topCharacters.Add($"{character} - {winRate}");
            }
            return topCharacters;
        }

        public List<string> GetGlobalBot3CharacterWinRate()
        {
            var botWinRate = new List<string>();
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CharacterPlayed, ROUND(CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / COUNT(*), 2) || '%' AS WinRate FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID GROUP BY CharacterPlayed HAVING COUNT(*) >= 3 ORDER BY CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / COUNT(*) ASC LIMIT 3;\r\n";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string character = reader.GetString(0);
                String winRate = reader.GetString(1);
                botWinRate.Add($"{character} - {winRate}");
            }

            return botWinRate;
        }

        public List<string> GetGlobalTop3CharacterUsage()
        {
            var topUsage = new List<string>();
            using var conn = new SqliteConnection(connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT MapPlayed, ROUND(CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / COUNT(*), 2) || '%' AS WinRate FROM SlippiGameData JOIN SlippiSubmissions ON SlippiSubmissions.SLPFileID = SlippiGameData.SLPFileID GROUP BY MapPlayed ORDER BY CAST(SUM(CASE WHEN WinLoss = 'Win' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / COUNT(*) DESC LIMIT 3;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string character = reader.GetString(0);
                string usage = reader.GetString(1);
                topUsage.Add($"{character} - {usage}");
            }

            return topUsage;
        }
    }
}