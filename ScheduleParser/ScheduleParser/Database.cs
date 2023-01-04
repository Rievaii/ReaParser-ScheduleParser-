using System;
using System.Data.SQLite;

namespace ScheduleParser
{
    internal class Database
    {
        private SQLiteConnection sqlite_conn = new SQLiteConnection(@"Data Source=C:\Program Files\SQLiteStudio\schedulebot");
        private SQLiteDataReader sqlite_datareader;
        private SQLiteCommand sqlite_cmd;
        private string _UserId;
        private string _UserGroup;

        public string isRegistred(string UserId)
        {
            try
            {
                sqlite_conn.Open();

                sqlite_cmd = sqlite_conn.CreateCommand();

                //get exact group number with user id 
                sqlite_cmd.CommandText = $"SELECT * FROM users WHERE vkid = '{UserId}';";

                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    int myreader = sqlite_datareader.GetInt32(0);
                    _UserId = sqlite_datareader.GetString(1);
                    _UserGroup = sqlite_datareader.GetString(2);
                    Console.WriteLine("Запрошенный пользователь: \nid: " + myreader + "\n vkid: " + _UserId + "\n groupid: " + _UserGroup);
                }
                sqlite_conn.Close();
                if (_UserGroup != null && _UserId != null)
                {
                    return _UserGroup;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                Console.WriteLine("Невозможно получить доступ к базе данных / Не найдены записи");
                return "";
            }
            finally
            {
                sqlite_conn.Close();
            }
        }

        public void AddUser(string UserId, string UserGroup)
        {

            sqlite_conn.Open();

            sqlite_cmd = sqlite_conn.CreateCommand();

            sqlite_cmd.CommandText = $"INSERT INTO users (vkid, groupid) VALUES ({UserId}, '{UserGroup}');";
            //check if record is ok
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();
        }
    }
}
