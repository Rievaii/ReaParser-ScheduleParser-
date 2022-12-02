using System.Data.SQLite;
using System;

namespace ScheduleParser
{
    internal class Database
    {
        


       

        public void ReadData()
        {
            SQLiteConnection conn;
            // Create a new database connection:
            conn = new SQLiteConnection(@"Data Source=C:\Program Files\SQLiteStudio\schedulebot");
            // Open the connection:
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {

            }
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM users";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                int myreader = sqlite_datareader.GetInt32(0);
                string UserId = sqlite_datareader.GetString(1);
                string UserGroup = sqlite_datareader.GetString(2);
                Console.WriteLine("\nid"+myreader+"\nUserID: "+UserId +"\n UserGroup: "+UserGroup);
                Console.Read();
            }
            conn.Close();
        }
    }
}
