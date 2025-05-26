using Microsoft.Data.Sqlite;

namespace FramePeak.Data
{
    public class FramePeakRepository
    {
        private readonly string _connectionString = "Data Source=App_Data/FramePeak.db";

        public bool ValidateUser(string username, string password)
        {
            using var conn = new SqliteConnection(_connectionString);
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

            using var conn = new SqliteConnection(_connectionString);
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
            using var conn = new SqliteConnection(_connectionString);
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
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM SlippiSubmissions WHERE UserID = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

    }
}